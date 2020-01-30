namespace Arcadia.Assistant.PushNotificationsDeviceRegistrator.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    public class PushNotificationsDeviceRegistrationActorFactory : IPushNotificationsDeviceRegistrationActorFactory
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public PushNotificationsDeviceRegistrationActorFactory(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        public IPushNotificationsDeviceRegistrationActor PushNotificationsDeviceRegistrator()
        {
            var actor = this.actorProxyFactory.CreateActorProxy<IPushNotificationsDeviceRegistrationActor>(new ActorId("push-device-registrator"), serviceName: "PushNotificationsDeviceRegistrator");
            return actor;
        }
    }
}