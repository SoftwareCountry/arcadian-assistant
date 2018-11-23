namespace Arcadia.Assistant.Notifications.Abstractions
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Routing;

    public class NotificationsActor : UntypedActor
    {
        private readonly IActorRef notificationsRouterActor;

        public NotificationsActor(params IActorRef[] notificationsActors)
        {
            var notificationsPools = this.CreateNotificationsPools(notificationsActors);
            this.notificationsRouterActor = this.CreateNotificationsRouter(notificationsPools);

            Context.System.EventStream.Subscribe<NotificationEventBusMessage>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case NotificationEventBusMessage msg:
                    this.notificationsRouterActor.Tell(msg.Payload);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private IEnumerable<IActorRef> CreateNotificationsPools(IEnumerable<IActorRef> notificationsActors)
        {
            return notificationsActors;
        }

        private IActorRef CreateNotificationsRouter(IEnumerable<IActorRef> notificationsActors)
        {
            var router = new BroadcastGroup(notificationsActors.Select(actor => actor.Path.ToString()));
            var notificationsRouterProps = Props
                .Empty
                .WithRouter(router);
            return Context.ActorOf(notificationsRouterProps);
        }
    }
}