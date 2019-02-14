namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    using EmailNotification = Arcadia.Assistant.Notifications.Email.EmailNotification;

    public class EventAssignedToApproverEmailNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IEmailNotification emailNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventAssignedToApproverEmailNotificationActor(
            IEmailNotification emailNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor)
        {
            this.emailNotificationConfig = emailNotificationConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApprover>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventAssignedToApprover msg:
                    this.GetAdditionalData(msg)
                        .ContinueWith(task =>
                        {
                            var (ownerEmployeeResult, approverPreferencesResult, approverEmployeeResult) = task.Result;

                            return new CalendarEventAssignedWithAdditionalData(
                                msg.Event,
                                ownerEmployeeResult.Employees.First().Metadata,
                                approverPreferencesResult.UserPreferences,
                                approverEmployeeResult.Employees.First().Metadata);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventAssignedWithAdditionalData msg
                    when msg.ApproverUserPreferences.EmailNotifications:

                    this.logger.Debug("Sending email notification about event {0} of {1} assigned to {2}",
                        msg.Event.EventId, msg.Owner.EmployeeId, msg.Approver.EmployeeId);

                    var datesStr = msg.Event.Dates.StartDate == msg.Event.Dates.EndDate
                        ? msg.Event.Dates.StartDate.ToString("d")
                        : $"{msg.Event.Dates.StartDate:d} - {msg.Event.Dates.EndDate:d}";

                    var sender = this.emailNotificationConfig.NotificationSender;
                    var recipient = msg.Approver.Email;
                    var subject = this.emailNotificationConfig.Subject;
                    var body = this.emailNotificationConfig.Body
                        .Replace("{eventType}", msg.Event.Type)
                        .Replace("{dates}", datesStr)
                        .Replace("{employee}", msg.Owner.Name);

                    Context.System.EventStream.Publish(
                        new NotificationEventBusMessage(
                            new EmailNotification(sender, new[] { recipient }, subject, body)));

                    break;

                case CalendarEventAssignedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(EmployeesQuery.Response, GetUserPreferencesMessage.Response, EmployeesQuery.Response)>
            GetAdditionalData(CalendarEventAssignedToApprover message)
        {
            var ownerEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(message.Event.EmployeeId));
            var approverPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.ApproverId));
            var approverEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(message.ApproverId));

            await Task.WhenAll(ownerEmployeeTask, approverPreferencesTask, approverEmployeeTask);
            return (ownerEmployeeTask.Result, approverPreferencesTask.Result, approverEmployeeTask.Result);
        }

        private class CalendarEventAssignedWithAdditionalData
        {
            public CalendarEventAssignedWithAdditionalData(
                CalendarEvent @event,
                EmployeeMetadata owner,
                UserPreferences approverUserPreferences,
                EmployeeMetadata approver)
            {
                this.Event = @event;
                this.Owner = owner;
                this.ApproverUserPreferences = approverUserPreferences;
                this.Approver = approver;
            }

            public CalendarEvent Event { get; }

            public EmployeeMetadata Owner { get; }

            public UserPreferences ApproverUserPreferences { get; }

            public EmployeeMetadata Approver { get; }
        }
    }
}