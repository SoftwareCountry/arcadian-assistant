namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Notifications;
    using Arcadia.Assistant.CSP.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    public class SickLeaveEndingReminderActor : UntypedActor, ILogReceive
    {
        private const string UserPreferencesActorPath = "/user/user-preferences";
        private const string PushDevicesActorPath = "/user/push-notifications-devices";
        private const string OrganizationActorPath = "/user/organization";

        private const string SickLeaveReminderPushNotificationType = "SickLeaveEndingReminder";

        private readonly SickLeavesAccountingReminderConfiguration reminderConfiguration;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly ActorSelection userPreferencesActor;
        private readonly ActorSelection pushDevicesActor;
        private readonly ActorSelection organizationActor;

        private readonly Dictionary<string, CalendarEvent> sickLeavesToRemind = new Dictionary<string, CalendarEvent>();

        public SickLeaveEndingReminderActor(SickLeavesAccountingReminderConfiguration reminderConfiguration)
        {
            this.reminderConfiguration = reminderConfiguration;

            this.userPreferencesActor = Context.ActorSelection(UserPreferencesActorPath);
            this.pushDevicesActor = Context.ActorSelection(PushDevicesActorPath);
            this.organizationActor = Context.ActorSelection(OrganizationActorPath);

            Context.System.Scheduler.ScheduleTellRepeatedly(
                this.GetInitialSchedulerDelay(),
                TimeSpan.FromDays(1),
                this.Self,
                RemindSickLeaves.Instance,
                this.Self);

            Context.System.EventStream.Subscribe<CalendarEventCreated>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<SickLeaveEndingReminderActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventCreated msg when
                    msg.Event.Type == CalendarEventTypes.Sickleave:

                    this.OnEventReceived(msg.Event);
                    break;

                case CalendarEventCreated _:
                    break;

                case CalendarEventChanged msg when
                    msg.NewEvent.Type == CalendarEventTypes.Sickleave:

                    this.OnEventReceived(msg.NewEvent);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventRecoverComplete msg when
                    msg.Event.Type == CalendarEventTypes.Sickleave:

                    this.OnEventReceived(msg.Event, true);
                    break;

                case CalendarEventRecoverComplete _:
                    break;

                case RemindSickLeaves _:
                    this.OnRemindSickLeave(this.sickLeavesToRemind.Values);
                    break;

                case RemindSickLeaves.Success msg:
                    if (msg.Notifications.Any())
                    {
                        this.logger.Debug($"Sending reminder notifications about ending sick leaves of employee {msg.EmployeeId}");
                    }

                    foreach (var notification in msg.Notifications)
                    {
                        Context.System.EventStream.Publish(new NotificationEventBusMessage(notification));
                    }

                    break;

                case RemindSickLeaves.Error msg:
                    this.logger.Error(
                        "Error occured on send reminder notifications about ending sick leaves " +
                        $"of employee {msg.EmployeeId}: {msg.Exception}");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnEventReceived(CalendarEvent @event, bool isRecovered = false)
        {
            if (@event.Status != SickLeaveStatuses.Requested)
            {
                if (this.sickLeavesToRemind.ContainsKey(@event.EventId))
                {
                    this.sickLeavesToRemind.Remove(@event.EventId);
                }

                return;
            }

            this.sickLeavesToRemind[@event.EventId] = @event;

            if (!isRecovered)
            {
                this.OnRemindSickLeave(new[] { @event });
            }
        }

        private void OnRemindSickLeave(IEnumerable<CalendarEvent> sickLeaves)
        {
            var sickLeavesByEmployee = sickLeaves.GroupBy(v => v.EmployeeId);

            foreach (var group in sickLeavesByEmployee)
            {
                this.GetNotifications(group.Key, group.ToArray())
                    .PipeTo(
                        this.Self,
                        success: result => new RemindSickLeaves.Success(group.Key, result),
                        failure: err => new RemindSickLeaves.Error(group.Key, err));
            }
        }

        private async Task<IEnumerable<object>> GetNotifications(string employeeId, IEnumerable<CalendarEvent> sickLeaves)
        {
            var userPreferencesResponse = await this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(employeeId));

            var sickLeavesArray = sickLeaves
                .Where(s => s.Dates.EndDate <= DateTime.Now.Date)
                .ToArray();

            var pushNotificationsTask = this.GetPushNotifications(
                employeeId,
                userPreferencesResponse.UserPreferences,
                sickLeavesArray);

            var emailNotificationsTask = this.GetEmailNotifications(
                employeeId,
                userPreferencesResponse.UserPreferences,
                sickLeavesArray);

            var result = await Task.WhenAll(pushNotificationsTask, emailNotificationsTask);
            return result.SelectMany(x => x);
        }

        private async Task<IEnumerable<object>> GetPushNotifications(
            string employeeId,
            UserPreferences userPreferences,
            IReadOnlyCollection<CalendarEvent> sickLeaves)
        {
            if (!userPreferences.PushNotifications || sickLeaves.Count == 0)
            {
                return Enumerable.Empty<PushNotification>();
            }

            var pushTokensResponse = await this.pushDevicesActor.Ask<GetDevicePushTokensByEmployee.Success>(
                new GetDevicePushTokensByEmployee(employeeId));

            return sickLeaves.Select(e => this.CreatePushNotification(employeeId, e, pushTokensResponse.DevicePushTokens));
        }

        private PushNotification CreatePushNotification(
            string employeeId,
            CalendarEvent @event,
            IEnumerable<DevicePushToken> deviceTokens)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["startDate"] = @event.Dates.StartDate.ToString("dd/MM/yyyy"),
                ["endDate"] = @event.Dates.EndDate.ToString("dd/MM/yyyy")
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                @event.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            var content = new PushNotificationContent
            {
                Title = this.reminderConfiguration.ReminderPush.Title,
                Body = new TemplateExpressionParser().Parse(this.reminderConfiguration.ReminderPush.Body, templateExpressionContext),
                CustomData = new
                {
                    @event.EventId,
                    EmployeeId = employeeId,
                    Type = SickLeaveReminderPushNotificationType
                }
            };

            return new PushNotification(content, deviceTokens.ToList());
        }

        private async Task<IEnumerable<object>> GetEmailNotifications(
            string employeeId,
            UserPreferences userPreferences,
            IReadOnlyCollection<CalendarEvent> sickLeaves)
        {
            if (!userPreferences.EmailNotifications || sickLeaves.Count == 0)
            {
                return Enumerable.Empty<EmailNotification>();
            }

            var employeeResponse = await this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(employeeId));
            var employeeMetadata = employeeResponse.Employees.First().Metadata;

            return sickLeaves.Select(e => this.CreateEmailNotification(e, employeeMetadata));
        }

        private EmailNotification CreateEmailNotification(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["startDate"] = @event.Dates.StartDate.ToString("dd/MM/yyyy"),
                ["endDate"] = @event.Dates.EndDate.ToString("dd/MM/yyyy")
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                @event.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            var sender = this.reminderConfiguration.ReminderEmail.NotificationSender;
            var recipient = employeeMetadata.Email;
            var subject = this.reminderConfiguration.ReminderEmail.Subject;
            var body = new TemplateExpressionParser().Parse(this.reminderConfiguration.ReminderEmail.Body, templateExpressionContext);

            return new EmailNotification(sender, new[] { recipient }, subject, body);
        }

        private TimeSpan GetInitialSchedulerDelay()
        {
            var now = DateTime.Now;

            var reminderDate = DateTime.Parse(this.reminderConfiguration.DailyRemindTime);
            if (now > reminderDate)
            {
                reminderDate = reminderDate.AddDays(1);
            }

            return reminderDate - now;
        }

        private class RemindSickLeaves
        {
            public static readonly RemindSickLeaves Instance = new RemindSickLeaves();

            public class Success
            {
                public Success(string employeeId, IEnumerable<object> notifications)
                {
                    this.EmployeeId = employeeId;
                    this.Notifications = notifications;
                }

                public string EmployeeId { get; }

                public IEnumerable<object> Notifications { get; }
            }

            public class Error
            {
                public Error(string employeeId, Exception exception)
                {
                    this.EmployeeId = employeeId;
                    this.Exception = exception;
                }

                public string EmployeeId { get; }

                public Exception Exception { get; }
            }
        }
    }
}