namespace Arcadia.Assistant.Notifications
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Routing;

    public class NotificationsActor : UntypedActor
    {
        private const int NotificationsActorsPoolSize = 5; // Later can be defined in config

        private readonly IActorRef notificationsRouterActor;

        public NotificationsActor(params Props[] notificationsActorsProps)
        {
            var notificationsPools = this.CreateNotificationsPools(notificationsActorsProps);
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

        private IEnumerable<IActorRef> CreateNotificationsPools(IEnumerable<Props> notificationsActorsProps)
        {
            return notificationsActorsProps.Select(props =>
            {
                var pool = new SmallestMailboxPool(NotificationsActorsPoolSize);
                var notificationsPoolProps = props.WithRouter(pool);
                return Context.ActorOf(notificationsPoolProps);
            });
        }

        private IActorRef CreateNotificationsRouter(IEnumerable<IActorRef> notificationsActors)
        {
            var group = new BroadcastGroup(notificationsActors.Select(actor => actor.Path.ToString()));
            var notificationsRouterProps = Props
                .Empty
                .WithRouter(group);
            return Context.ActorOf(notificationsRouterProps);
        }
    }
}