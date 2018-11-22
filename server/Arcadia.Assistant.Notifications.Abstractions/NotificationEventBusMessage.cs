namespace Arcadia.Assistant.Notifications.Abstractions
{
    public class NotificationEventBusMessage
    {
        public NotificationEventBusMessage(object payload)
        {
            this.Payload = payload;
        }

        public object Payload { get; }
    }
}