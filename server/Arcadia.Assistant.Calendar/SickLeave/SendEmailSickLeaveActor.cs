namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Organization.Abstractions;

    public class SendEmailSickLeaveActor : UntypedActor
    {
        private readonly IEmailSettings mailConfig;

        private readonly ILoggingAdapter logger = Context.GetLogger();
        private Dictionary<string, EmployeeMetadata> employeesById;

        public SendEmailSickLeaveActor(IEmailSettings mailConfig)
        {
            this.mailConfig = mailConfig;

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<EmployeeMetadataUpdatedEventBusMessage>(this.Self);
            Context.System.EventStream.Subscribe<EmployeesMetadataLoadedEventBusMessage>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg when msg.NewEvent.Type == CalendarEventTypes.Sickleave:
                    if (this.employeesById == null || !this.employeesById.TryGetValue(msg.NewEvent.EmployeeId, out var employee))
                    {
                        this.logger.Warning(
                            "Sick leave email notification cannot be sent. Employee {0} is not loaded yet.",
                            msg.NewEvent.EmployeeId);
                        return;
                    }

                    this.logger.Debug("Sending a sick leave email notification for user {0}", msg.NewEvent.EmployeeId);

                    var sender = this.mailConfig.NotificationSender;
                    var recipient = this.mailConfig.NotificationRecipient;
                    var subject = this.mailConfig.Subject;
                    var body = string.Format(this.mailConfig.Body, employee.Name, msg.NewEvent.Dates.StartDate.ToString("D"));

                    Context.System.EventStream.Publish(new NotificationEventBusMessage(new EmailNotification(sender, recipient, subject, body)));

                    break;

                case CalendarEventChanged _:
                    break;

                case EmployeesMetadataLoadedEventBusMessage msg:
                    this.employeesById = msg.Employees
                        .ToDictionary(e => e.Metadata.EmployeeId, e => e.Metadata);
                    break;

                case EmployeeMetadataUpdatedEventBusMessage msg:
                    this.employeesById[msg.EmployeeMetadata.EmployeeId] = msg.EmployeeMetadata;
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}