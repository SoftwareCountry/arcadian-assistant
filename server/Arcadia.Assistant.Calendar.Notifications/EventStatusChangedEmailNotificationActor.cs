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

    public class EventStatusChangedEmailNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IEmailNotification emailNotificationConfig;
        private readonly IActorRef organizationActor;
        private readonly IActorRef userPreferencesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventStatusChangedEmailNotificationActor(
            IEmailNotification emailNotificationConfig,
            IActorRef organizationActor,
            IActorRef userPreferencesActor)
        {
            this.emailNotificationConfig = emailNotificationConfig;
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

                    var datesStr = msg.Event.Dates.StartDate == msg.Event.Dates.EndDate
                        ? msg.Event.Dates.StartDate.ToString("d")
                        : $"{msg.Event.Dates.StartDate:d} - {msg.Event.Dates.EndDate:d}";

                    var sender = this.emailNotificationConfig.NotificationSender;
                    var recipient = msg.Owner.Email;
                    var subject = this.emailNotificationConfig.Subject;
                    var body = this.emailNotificationConfig.Body
                        .Replace("{eventType}", msg.Event.Type)
                        .Replace("{dates}", datesStr)
                        .Replace("{eventStatus}", msg.Event.Status);

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