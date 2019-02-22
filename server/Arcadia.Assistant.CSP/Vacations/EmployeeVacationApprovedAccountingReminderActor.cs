namespace Arcadia.Assistant.CSP.Vacations
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
    using Arcadia.Assistant.CSP.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.UserPreferences;

    public class EmployeeVacationApprovedAccountingReminderActor : UntypedActor, ILogReceive
    {
        private const string UserPreferencesActorPath = "/user/user-preferences";
        private const string PushDevicesActorPath = "/user/push-notifications-devices";

        private const string VacationReminderPushNotificationType = "VacationApprovedReminder";

        private readonly string employeeId;
        private readonly AccountingReminderConfiguration reminderConfiguration;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly ActorSelection userPreferences;
        private readonly ActorSelection pushDevices;

        private readonly Dictionary<string, CalendarEvent> vacationsToRemind = new Dictionary<string, CalendarEvent>();

        public EmployeeVacationApprovedAccountingReminderActor(string employeeId, AccountingReminderConfiguration reminderConfiguration)
        {
            this.employeeId = employeeId;
            this.reminderConfiguration = reminderConfiguration;

            this.userPreferences = Context.ActorSelection(UserPreferencesActorPath);
            this.pushDevices = Context.ActorSelection(PushDevicesActorPath);

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
                    this.SendReminderNotifications()
                        .PipeTo(
                            this.Self,
                            success: result => new RemindVacations.Success(result),
                            failure: err => new RemindVacations.Error(err));
                    break;

                case RemindVacations.Success msg:
                    if (msg.PushNotifications.Any())
                    {
                        this.logger.Debug($"Sending push reminder notifications about approved vacations of employee {this.employeeId}");
                    }

                    foreach (var pushNotification in msg.PushNotifications)
                    {
                        Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));
                    }

                    break;

                case RemindVacations.Error msg:
                    this.logger.Error(
                        "Error occured on send push reminder notifications about approved vacations " +
                        $"of employee {this.employeeId}: {msg.Exception.Message}");
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

        private async Task<IEnumerable<PushNotification>> SendReminderNotifications()
        {
            var ownerPreferencesResponse = await this.userPreferences.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(this.employeeId));
            var pushTokensResponse = await this.pushDevices.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(this.employeeId));

            if (!ownerPreferencesResponse.UserPreferences.PushNotifications)
            {
                return Enumerable.Empty<PushNotification>();
            }

            return this.vacationsToRemind.Values
                .Select(e => this.CreatePushNotification(e, pushTokensResponse.DevicePushTokens));
        }

        private PushNotification CreatePushNotification(CalendarEvent @event, IEnumerable<DevicePushToken> deviceTokens)
        {
            var content = new PushNotificationContent
            {
                Title = this.reminderConfiguration.Reminder.Title,
                Body = this.reminderConfiguration.Reminder.Body
                    .Replace("{startDate}", @event.Dates.StartDate.ToString("d"))
                    .Replace("{endDate}", @event.Dates.EndDate.ToString("d")),
                CustomData = new
                {
                    @event.EventId,
                    EmployeeId = this.employeeId,
                    Type = VacationReminderPushNotificationType
                }
            };

            return new PushNotification(content, deviceTokens.ToList());
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
                public Success(IEnumerable<PushNotification> pushNotifications)
                {
                    this.PushNotifications = pushNotifications;
                }

                public IEnumerable<PushNotification> PushNotifications { get; }
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