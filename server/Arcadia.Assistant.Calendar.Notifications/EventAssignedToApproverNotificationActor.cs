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
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.UserPreferences;

    public class EventAssignedToApproverNotificationActor : UntypedActor, ILogReceive
    {
        private readonly EmailSettings mailConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventAssignedToApproverNotificationActor(
            EmailSettings mailConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor)
        {
            this.mailConfig = mailConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApprover>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventAssignedToApprover msg when msg.ApproverId != null:
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

                case CalendarEventAssignedToApprover _:
                    break;

                case CalendarEventAssignedWithAdditionalData msg
                    when msg.ApproverUserPreferences.EmailNotifications:

                    this.logger.Debug("Sending notification about event {0} of {1} assigned to {2}",
                        msg.Event.EventId, msg.Owner.Name, msg.Approver.Name);

                    var sender = this.mailConfig.NotificationSender;
                    var recipient = msg.Approver.Email;
                    var subject = this.mailConfig.Subject;
                    var body = string.Format(this.mailConfig.Body, msg.Event.Type, msg.Owner.Name);

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