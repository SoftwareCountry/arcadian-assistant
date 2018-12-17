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

    public class EventStatusChangedEmailNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IEmailSettings mailConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventStatusChangedEmailNotificationActor(
            IEmailSettings mailConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor)
        {
            this.mailConfig = mailConfig;
            this.organizationActor = organizationActor;
            this.userPreferencesActor = userPreferencesActor;

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg
                    when msg.NewEvent.Status != msg.OldEvent.Status && msg.UpdatedBy != msg.NewEvent.EmployeeId:

                    this.GetAdditionalData(msg)
                        .ContinueWith(task =>
                        {
                            var (employeeResult, preferencesResult) = task.Result;

                            return new CalendarEventChangedWithAdditionalData(
                                msg.NewEvent,
                                employeeResult.Employees.First().Metadata,
                                preferencesResult.UserPreferences);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventChangedWithAdditionalData msg
                    when msg.OwnerUserPreferences.EmailNotifications:

                    this.logger.Debug("Sending email notification about event {0} status changed to owner", msg.Event.EventId);

                    var sender = this.mailConfig.NotificationSender;
                    var recipient = msg.Owner.Email;
                    var subject = this.mailConfig.Subject;
                    var body = string.Format(this.mailConfig.Body, msg.Event.Type, msg.Event.Status);

                    Context.System.EventStream.Publish(
                        new NotificationEventBusMessage(
                            new EmailNotification(sender, new[] { recipient }, subject, body)));

                    break;

                case CalendarEventChangedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(EmployeesQuery.Response, GetUserPreferencesMessage.Response)>
            GetAdditionalData(CalendarEventChanged message)
        {
            var ownerEmployeeTask = this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(message.NewEvent.EmployeeId));
            var ownerPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.NewEvent.EmployeeId));

            await Task.WhenAll(ownerEmployeeTask, ownerPreferencesTask);
            return (ownerEmployeeTask.Result, ownerPreferencesTask.Result);
        }

        private class CalendarEventChangedWithAdditionalData
        {
            public CalendarEventChangedWithAdditionalData(CalendarEvent @event,
                EmployeeMetadata owner,
                UserPreferences ownerUserPreferences)
            {
                this.Event = @event;
                this.Owner = owner;
                this.OwnerUserPreferences = ownerUserPreferences;
            }

            public CalendarEvent Event { get; }

            public EmployeeMetadata Owner { get; }

            public UserPreferences OwnerUserPreferences { get; }
        }
    }
}