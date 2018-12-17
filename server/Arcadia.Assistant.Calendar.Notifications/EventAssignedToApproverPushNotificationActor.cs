namespace Arcadia.Assistant.Calendar.Notifications
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Notifications;
    using Arcadia.Assistant.Notifications.Push;
    using Arcadia.Assistant.UserPreferences;

    public class EventAssignedToApproverPushNotificationActor : UntypedActor, ILogReceive
    {
        private readonly IActorRef userPreferencesActor;
        private readonly IActorRef pushDevicesActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EventAssignedToApproverPushNotificationActor(
            IActorRef userPreferencesActor,
            IActorRef pushDevicesActor)
        {
            this.userPreferencesActor = userPreferencesActor;
            this.pushDevicesActor = pushDevicesActor;

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
                            var (approverPreferencesResult, approverPushTokens) = task.Result;

                            return new CalendarEventAssignedWithAdditionalData(
                                msg.Event,
                                msg.ApproverId,
                                approverPreferencesResult.UserPreferences,
                                approverPushTokens.DevicePushTokens);
                        })
                        .PipeTo(this.Self);

                    break;

                case CalendarEventAssignedToApprover _:
                    break;

                case CalendarEventAssignedWithAdditionalData msg
                    when msg.ApproverUserPreferences.PushNotifications:

                    this.logger.Debug("Sending push notification about event {0} of {1} assigned to {2}",
                        msg.Event.EventId, msg.Event.EmployeeId, msg.ApproverId);

                    var pushNotification = this.CreatePushNotification(msg);

                    Context.System.EventStream.Publish(new NotificationEventBusMessage(pushNotification));

                    break;

                case CalendarEventAssignedWithAdditionalData _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async
            Task<(GetUserPreferencesMessage.Response, GetDevicePushTokens.Success)>
            GetAdditionalData(CalendarEventAssignedToApprover message)
        {
            var approverPreferencesTask = this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(message.ApproverId));
            var approverPushTokensTask = this.pushDevicesActor.Ask<GetDevicePushTokens.Success>(
                new GetDevicePushTokens(message.ApproverId));

            await Task.WhenAll(approverPreferencesTask, approverPushTokensTask);
            return (approverPreferencesTask.Result, approverPushTokensTask.Result);
        }

        private PushNotification CreatePushNotification(CalendarEventAssignedWithAdditionalData message)
        {
            return new PushNotification
            {
                Content = new PushNotificationContent
                {
                    Title = "Test title",
                    Body = "Test body",
                    CustomData = new
                    {
                        message.Event.EventId,
                        message.Event.EmployeeId,
                        Type = CalendarEventPushNotificationTypes.EventAssignedToApprover
                    }
                },
                Target = new PushNotificationTarget
                {
                    DevicePushTokens = message.ApproverPushTokens.ToList()
                }
            };
        }

        private class CalendarEventAssignedWithAdditionalData
        {
            public CalendarEventAssignedWithAdditionalData(
                CalendarEvent @event,
                string approverId,
                UserPreferences approverUserPreferences,
                IEnumerable<string> approverPushTokens)
            {
                this.Event = @event;
                this.ApproverId = approverId;
                this.ApproverUserPreferences = approverUserPreferences;
                this.ApproverPushTokens = approverPushTokens;
            }

            public CalendarEvent Event { get; }

            public string ApproverId { get; }

            public UserPreferences ApproverUserPreferences { get; }

            public IEnumerable<string> ApproverPushTokens { get; }
        }
    }
}