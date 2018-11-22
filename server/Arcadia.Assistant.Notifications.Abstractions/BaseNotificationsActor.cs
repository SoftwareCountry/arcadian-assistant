namespace Arcadia.Assistant.Notifications.Abstractions
{
    using Akka.Actor;
    using Akka.Event;

    public abstract class BaseNotificationsActor : UntypedActor
    {
        protected BaseNotificationsActor()
        {
            Context.System.EventStream.Subscribe<NotificationEventBusMessage>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case NotificationEventBusMessage msg when msg.Payload == null:
                    break;

                case NotificationEventBusMessage msg:
                    this.HandleNotificationPayload(msg.Payload);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract void HandleNotificationPayload(object payload);
    }
}