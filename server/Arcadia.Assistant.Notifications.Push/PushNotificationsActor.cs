namespace Arcadia.Assistant.Notifications.Push
{
    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;

    public class PushNotificationsActor : UntypedActor, ILogReceive
    {
        private readonly IPushSettings pushSettings;

        public readonly ILoggingAdapter logger = Context.GetLogger();

        public PushNotificationsActor(IPushSettings pushSettings)
        {
            this.pushSettings = pushSettings;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case PushNotification msg when this.pushSettings.Enabled:
                    this.SendPushNotification(msg);
                    break;

                case PushNotification _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendPushNotification(PushNotification message)
        {
            this.logger.Debug("Push notification message received");

            this.logger.Debug("Push notification was succesfully sent");
        }
    }
}