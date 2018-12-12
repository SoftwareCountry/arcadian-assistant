namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class SickLeaveApprovedNotificationActor : UntypedActor
    {
        private readonly EmailWithFixedRecipientSettings mailConfig;
        private readonly IActorRef organizationActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SickLeaveApprovedNotificationActor(ICalendarEventsMessagingSettings mailConfig, IActorRef organizationActor)
        {
            this.mailConfig = mailConfig.SickLeaveApproved;
            this.organizationActor = organizationActor;

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg when msg.NewEvent.Type == CalendarEventTypes.Sickleave:
                    this.organizationActor
                        .Ask<EmployeesQuery.Response>(EmployeesQuery.Create().WithId(msg.NewEvent.EmployeeId))
                        .ContinueWith(task => new CalendarEventChangedWithAdditionalData(msg.NewEvent, task.Result.Employees.FirstOrDefault()?.Metadata))
                        .PipeTo(this.Self);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventChangedWithAdditionalData msg:
                    this.logger.Debug("Sending a sick leave email notification for user {0}",
                        msg.Event.EmployeeId);

                    var sender = this.mailConfig.NotificationSender;
                    var recipient = this.mailConfig.NotificationRecipient;
                    var subject = this.mailConfig.Subject;
                    var body = string.Format(
                        this.mailConfig.Body,
                        msg.Employee.Name,
                        msg.Event.Dates.StartDate.ToString("D"));

                    Context.System.EventStream.Publish(
                        new NotificationEventBusMessage(
                            new EmailNotification(sender, new[] { recipient }, subject, body)));

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private class CalendarEventChangedWithAdditionalData
        {
            public CalendarEventChangedWithAdditionalData(CalendarEvent @event, EmployeeMetadata employee)
            {
                this.Event = @event;
                this.Employee = employee;
            }

            public CalendarEvent Event { get; }

            public EmployeeMetadata Employee { get; }
        }
    }
}