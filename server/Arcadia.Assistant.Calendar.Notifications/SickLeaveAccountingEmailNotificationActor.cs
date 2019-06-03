namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    using EmailNotification = Arcadia.Assistant.Notifications.Email.EmailNotification;

    public class SickLeaveAccountingEmailNotificationActor : UntypedActor
    {
        private readonly IEmailWithFixedRecipientNotification createdEmailNotificationConfig;
        private readonly IEmailWithFixedRecipientNotification prolongedEmailNotificationConfig;
        private readonly IEmailWithFixedRecipientNotification cancelledEmailNotificationConfig;
        private readonly IActorRef organizationActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SickLeaveAccountingEmailNotificationActor(
            IEmailWithFixedRecipientNotification createdEmailNotificationConfig,
            IEmailWithFixedRecipientNotification prolongedEmailNotificationConfig,
            IEmailWithFixedRecipientNotification cancelledEmailNotificationConfig,
            IActorRef organizationActor)
        {
            this.createdEmailNotificationConfig = createdEmailNotificationConfig;
            this.prolongedEmailNotificationConfig = prolongedEmailNotificationConfig;
            this.cancelledEmailNotificationConfig = cancelledEmailNotificationConfig;
            this.organizationActor = organizationActor;

            Context.System.EventStream.Subscribe<CalendarEventCreated>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventCreated msg when
                    msg.Event.Type == CalendarEventTypes.Sickleave:

                    this.organizationActor
                        .Ask<EmployeesQuery.Response>(EmployeesQuery.Create().WithId(msg.Event.EmployeeId))
                        .ContinueWith(task => new CalendarEventChangedWithAdditionalData(msg.Event, NotificationType.Created, task.Result.Employees.FirstOrDefault()?.Metadata))
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged msg when
                    msg.NewEvent.Type == CalendarEventTypes.Sickleave &&
                    msg.NewEvent.Status == SickLeaveStatuses.Requested &&
                    msg.OldEvent.Status == SickLeaveStatuses.Requested &&
                    msg.OldEvent.Dates.EndDate != msg.NewEvent.Dates.EndDate:

                    this.organizationActor
                        .Ask<EmployeesQuery.Response>(EmployeesQuery.Create().WithId(msg.NewEvent.EmployeeId))
                        .ContinueWith(task => new CalendarEventChangedWithAdditionalData(msg.NewEvent, NotificationType.Prolonged, task.Result.Employees.FirstOrDefault()?.Metadata))
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged msg when
                    msg.NewEvent.Type == CalendarEventTypes.Sickleave &&
                    msg.NewEvent.Status == SickLeaveStatuses.Cancelled:

                    this.organizationActor
                        .Ask<EmployeesQuery.Response>(EmployeesQuery.Create().WithId(msg.NewEvent.EmployeeId))
                        .ContinueWith(task => new CalendarEventChangedWithAdditionalData(msg.NewEvent, NotificationType.Cancelled, task.Result.Employees.FirstOrDefault()?.Metadata))
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventChangedWithAdditionalData msg:
                    var notificationAction = msg.NotificationType == NotificationType.Created
                        ? "created"
                        : msg.NotificationType == NotificationType.Prolonged
                            ? "prolonged"
                            : "cancelled";
                    this.logger.Debug($"Sending a sick leave {notificationAction} accounting email notification for user {msg.Event.EmployeeId}");

                    var notificationConfiguration = msg.NotificationType == NotificationType.Created
                        ? this.createdEmailNotificationConfig
                        : msg.NotificationType == NotificationType.Prolonged
                            ? this.prolongedEmailNotificationConfig
                            : this.cancelledEmailNotificationConfig;

                    this.SendNotification(msg, notificationConfiguration);

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendNotification(CalendarEventChangedWithAdditionalData message, IEmailWithFixedRecipientNotification notificationConfiguration)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["employee"] = message.Employee.Name,
                ["employeeId"] = message.Employee.EmployeeId,
                ["startDate"] = message.Event.Dates.StartDate.ToString("dd/MM/yyyy"),
                ["endDate"] = message.Event.Dates.EndDate.ToString("dd/MM/yyyy")
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                message.Event.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

            var templateExpressionParser = new TemplateExpressionParser();

            var sender = notificationConfiguration.NotificationSender;
            var recipient = notificationConfiguration.NotificationRecipient;
            var subject = templateExpressionParser.Parse(notificationConfiguration.Subject, templateExpressionContext);
            var body = templateExpressionParser.Parse(notificationConfiguration.Body, templateExpressionContext);

            Context.System.EventStream.Publish(
                new NotificationEventBusMessage(
                    new EmailNotification(sender, new[] { recipient }, subject, body)));
        }

        private class CalendarEventChangedWithAdditionalData
        {
            public CalendarEventChangedWithAdditionalData(CalendarEvent @event, NotificationType notificationType, EmployeeMetadata employee)
            {
                this.Event = @event;
                this.NotificationType = notificationType;
                this.Employee = employee;
            }

            public CalendarEvent Event { get; }

            public NotificationType NotificationType { get; }

            public EmployeeMetadata Employee { get; }
        }

        private enum NotificationType
        {
            Created,
            Prolonged,
            Cancelled
        }
    }
}