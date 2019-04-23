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
        private readonly IEmailWithFixedRecipientNotification approvedEmailNotificationConfig;
        private readonly IEmailWithFixedRecipientNotification prolongedEmailNotificationConfig;
        private readonly IActorRef organizationActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SickLeaveAccountingEmailNotificationActor(
            IEmailWithFixedRecipientNotification approvedEmailNotificationConfig,
            IEmailWithFixedRecipientNotification prolongedEmailNotificationConfig,
            IActorRef organizationActor)
        {
            this.approvedEmailNotificationConfig = approvedEmailNotificationConfig;
            this.prolongedEmailNotificationConfig = prolongedEmailNotificationConfig;
            this.organizationActor = organizationActor;

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg
                    when msg.NewEvent.Type == CalendarEventTypes.Sickleave &&
                    msg.NewEvent.Status == SickLeaveStatuses.Approved:

                    var isProlonged = msg.OldEvent.Status == SickLeaveStatuses.Approved;
                    var dateChanged = msg.OldEvent.Dates.EndDate != msg.NewEvent.Dates.EndDate;

                    if (isProlonged && !dateChanged)
                    {
                        return;
                    }

                    this.organizationActor
                        .Ask<EmployeesQuery.Response>(EmployeesQuery.Create().WithId(msg.NewEvent.EmployeeId))
                        .ContinueWith(task => new CalendarEventChangedWithAdditionalData(msg.NewEvent, isProlonged, task.Result.Employees.FirstOrDefault()?.Metadata))
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventChangedWithAdditionalData msg:
                    this.logger.Debug($"Sending a sick leave {(msg.IsProlonged ? "prolonged" : "approved")} accounting email notification for user {msg.Event.EmployeeId}");

                    var notificationConfiguration = msg.IsProlonged
                        ? this.prolongedEmailNotificationConfig
                        : this.approvedEmailNotificationConfig;

                    this.SendNotification(msg, notificationConfiguration);

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendNotification(CalendarEventChangedWithAdditionalData msg, IEmailWithFixedRecipientNotification notificationConfiguration)
        {
            var templateExpressionContext = new Dictionary<string, string>
            {
                ["employee"] = msg.Employee.Name,
                ["startDate"] = msg.Event.Dates.StartDate.ToString("dd/MM/yyyy"),
                ["endDate"] = msg.Event.Dates.EndDate.ToString("dd/MM/yyyy")
            };

            templateExpressionContext = new DictionaryMerge().Perform(
                templateExpressionContext,
                msg.Event.AdditionalData.ToDictionary(x => x.Key, x => x.Value));

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
            public CalendarEventChangedWithAdditionalData(CalendarEvent @event, bool isProlonged, EmployeeMetadata employee)
            {
                this.Event = @event;
                this.IsProlonged = isProlonged;
                this.Employee = employee;
            }

            public CalendarEvent Event { get; }

            public bool IsProlonged { get; }

            public EmployeeMetadata Employee { get; }
        }
    }
}