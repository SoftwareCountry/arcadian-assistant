namespace Arcadia.Assistant.PushNotificationsDistributor.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    public class PushNotificationsDistributionActorFactory : IPushNotificationsDistributionActorFactory
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public PushNotificationsDistributionActorFactory(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        public IPushNotificationsDistributionActor PushNotificationsDistributor(string type)
        {
            var actor = this.actorProxyFactory.CreateActorProxy<IPushNotificationsDistributionActor>(new ActorId($"push-{type}"), serviceName: "PushNotificationsDistribution");
            return actor;
        }
    }
}