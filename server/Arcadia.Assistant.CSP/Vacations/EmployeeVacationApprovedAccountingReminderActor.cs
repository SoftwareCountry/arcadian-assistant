namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.CSP.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    using EmailNotification = Notifications.Email.EmailNotification;
    using PushNotification = Notifications.Push.PushNotification;

    public class EmployeeVacationApprovedAccountingReminderActor : UntypedActor, ILogReceive
    {
        private const string UserPreferencesActorPath = "/user/user-preferences";
        private const string PushDevicesActorPath = "/user/push-notifications-devices";
        private const string OrganizationActorPath = "/user/organization";

        private const string VacationReminderPushNotificationType = "VacationApprovedReminder";

        private readonly string employeeId;
        private readonly AccountingReminderConfiguration reminderConfiguration;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly ActorSelection userPreferencesActor;
        private readonly ActorSelection pushDevicesActor;
        private readonly ActorSelection organizationActor;

        private readonly Dictionary<string, CalendarEvent> vacationsToRemind = new Dictionary<string, CalendarEvent>();

        public EmployeeVacationApprovedAccountingReminderActor(
            string employeeId,
            AccountingReminderConfiguration reminderConfiguration)
        {
            this.employeeId = employeeId;
            this.reminderConfiguration = reminderConfiguration;

            this.userPreferencesActor = Context.ActorSelection(UserPreferencesActorPath);
            this.pushDevicesActor = Context.ActorSelection(PushDevicesActorPath);
            this.organizationActor = Context.ActorSelection(OrganizationActorPath);

            Context.System.Scheduler.ScheduleTellRepeatedly(
                this.GetInitialSchedulerDelay(),
                TimeSpan.FromDays(1),
                this.Self,
                RemindVacations.Instance,
                this.Self);

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
        }

        public static Props CreateProps(string employeeId, AccountingReminderConfiguration reminderConfiguration)
        {
            return Props.Create(() => new EmployeeVacationApprovedAccountingReminderActor(
                employeeId,
                reminderConfiguration));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg when
                    msg.NewEvent.EmployeeId == this.employeeId &&
                    msg.NewEvent.Type == CalendarEventTypes.Vacation:

                    this.OnEventReceived(msg.NewEvent);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventRecoverComplete msg when
                    msg.Event.EmployeeId == this.employeeId &&
                    msg.Event.Type == CalendarEventTypes.Vacation:

                    this.OnEventReceived(msg.Event);
                    break;

                case CalendarEventRecoverComplete _:
                    break;

                case RemindVacations _:
                    this.GetNotifications()
                        .PipeTo(
                            this.Self,
                            success: result => new RemindVacations.Success(result),
                            failure: err => new RemindVacations.Error(err));
                    break;

                case RemindVacations.Success msg:
                    if (msg.Notifications.Any())
                    {
                        this.logger.Debug($"Sending reminder notifications about approved vacations of employee {this.employeeId}");
                    }

                    foreach (var notification in msg.Notifications)
                    {
                        Context.System.EventStream.Publish(new NotificationEventBusMessage(notification));
                    }

                    break;

                case RemindVacations.Error msg:
                    this.logger.Error(
                        "Error occured on send push reminder notifications about approved vacations " +
                        $"of employee {this.employeeId}: {msg.Exception}");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnEventReceived(CalendarEvent @event)
        {
            if (@event.Status != VacationStatuses.Approved)
            {
                if (this.vacationsToRemind.ContainsKey(@event.EventId))
                {
                    this.vacationsToRemind.Remove(@event.EventId);
                }

                return;
            }

            this.vacationsToRemind.Add(@event.EventId, @event);
        }

        private async Task<IEnumerable<object>> GetNotifications()
        {
            var userPreferencesResponse = await this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(this.employeeId));

            var pushNotificationsTask = this.GetPushNotifications(userPreferencesResponse.UserPreferences);
            var emailNotificationsTask = this.GetEmailNotifications(userPreferencesResponse.UserPreferences);

            var result = await Task.WhenAll(pushNotificationsTask, emailNotificationsTask);
            return result.SelectMany(x => x);
        }

        private async Task<IEnumerable<object>> GetPushNotifications(UserPreferences userPreferences)
        {
            if (!userPreferences.PushNotifications)
            {
                return Enumerable.Empty<PushNotification>();
            }

            var pushTokensResponse = await this.pushDevicesActor.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(this.employeeId));

            return this.vacationsToRemind.Values
                .Select(e => this.CreatePushNotification(e, pushTokensResponse.DevicePushTokens));
        }

        private PushNotification CreatePushNotification(CalendarEvent @event, IEnumerable<DevicePushToken> deviceTokens)
        {
            var content = new PushNotificationContent
            {
                Title = this.reminderConfiguration.ReminderPush.Title,
                Body = this.reminderConfiguration.ReminderPush.Body
                    .Replace("{startDate}", @event.Dates.StartDate.ToString("dd/MM/yyyy"))
                    .Replace("{endDate}", @event.Dates.EndDate.ToString("dd/MM/yyyy")),
                CustomData = new
                {
                    @event.EventId,
                    EmployeeId = this.employeeId,
                    Type = VacationReminderPushNotificationType
                }
            };

            return new PushNotification(content, deviceTokens.ToList());
        }

        private async Task<IEnumerable<object>> GetEmailNotifications(UserPreferences userPreferences)
        {
            if (!userPreferences.EmailNotifications)
            {
                return Enumerable.Empty<EmailNotification>();
            }

            var employeeResponse = await this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(this.employeeId));
            var employeeMetadata = employeeResponse.Employees.First().Metadata;

            return this.vacationsToRemind.Values
                .Select(e => this.CreateEmailNotification(e, employeeMetadata));
        }

        private EmailNotification CreateEmailNotification(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            var sender = this.reminderConfiguration.ReminderEmail.NotificationSender;
            var recipient = employeeMetadata.Email;
            var subject = this.reminderConfiguration.ReminderEmail.Subject;
            var body = this.reminderConfiguration.ReminderEmail.Body
                .Replace("{startDate}", @event.Dates.StartDate.ToString("dd/MM/yyyy"))
                .Replace("{endDate}", @event.Dates.EndDate.ToString("dd/MM/yyyy"));

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

        private class RemindVacations
        {
            public static readonly RemindVacations Instance = new RemindVacations();

            public class Success
            {
                public Success(IEnumerable<object> notifications)
                {
                    this.Notifications = notifications;
                }

                public IEnumerable<object> Notifications { get; }
            }

            public class Error
            {
                public Error(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }
    }
}