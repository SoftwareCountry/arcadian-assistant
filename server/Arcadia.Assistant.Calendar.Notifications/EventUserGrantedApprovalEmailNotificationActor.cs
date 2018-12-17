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

    public class EventUserGrantedApprovalEmailNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IEmailNotification emailNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventUserGrantedApprovalEmailNotificationActor(
            IEmailNotification emailNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor)
        {
            this.emailNotificationConfig = emailNotificationConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;

            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventApprovalsChanged msg when msg.Approvals.Count() != 0:
                    this.GetAdditionalData(msg)
                        .ContinueWith(task =>
                        {
                            var (ownerEmployeeResult, ownerPreferencesResult, approverEmployeeResult) = task.Result;

                            return new CalendarEventApprovalsChangedWithAdditionalData(
                                msg.Event,
                                ownerEmployeeResult.Employees.First().Metadata,
                                ownerPreferencesResult.UserPreferences,
                                approverEmployeeResult.Employees.First().Metadata);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventApprovalsChanged _:
                    break;

                case CalendarEventApprovalsChangedWithAdditionalData msg
                    when msg.OwnerUserPreferences.EmailNotifications:

                    this.logger.Debug("Sending email notification about user {0} granted approval for event {1} of {2}",
                        msg.Approver.EmployeeId, msg.Event.EventId, msg.Owner.EmployeeId);

                    var sender = this.emailNotificationConfig.NotificationSender;
                    var recipient = msg.Owner.Email;
                    var subject = this.emailNotificationConfig.Subject;
                    var body = string.Format(this.emailNotificationConfig.Body, msg.Event.Type, msg.Approver.Name);

                    Context.System.EventStream.Publish(
                        new NotificationEventBusMessage(
                            new EmailNotification(sender, new[] { recipient }, subject, body)));

                    break;

                case CalendarEventApprovalsChangedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(EmployeesQuery.Response, GetUserPreferencesMessage.Response, EmployeesQuery.Response)>
            GetAdditionalData(CalendarEventApprovalsChanged message)
        {
            var lastApproval = message.Approvals
                .OrderByDescending(a => a.Timestamp)
                .First();

            var ownerEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(message.Event.EmployeeId));
            var ownerPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.Event.EmployeeId));
            var approverEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(lastApproval.ApprovedBy));

            await Task.WhenAll(ownerEmployeeTask, ownerPreferencesTask, approverEmployeeTask);
            return (ownerEmployeeTask.Result, ownerPreferencesTask.Result, approverEmployeeTask.Result);
        }

        private class CalendarEventApprovalsChangedWithAdditionalData
        {
            public CalendarEventApprovalsChangedWithAdditionalData(
                CalendarEvent @event,
                EmployeeMetadata owner,
                UserPreferences ownerUserPreferences,
                EmployeeMetadata approver)
            {
                this.Event = @event;
                this.Owner = owner;
                this.OwnerUserPreferences = ownerUserPreferences;
                this.Approver = approver;
            }

            public CalendarEvent Event { get; }

            public EmployeeMetadata Owner { get; }

            public UserPreferences OwnerUserPreferences { get; }

            public EmployeeMetadata Approver { get; }
        }
    }
}