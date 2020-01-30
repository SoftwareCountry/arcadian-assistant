namespace Arcadia.Assistant.PushNotificationsDeviceRegistrator.Contracts
{
    using Autofac;

    public class PushNotificationsDeviceRegistratorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PushNotificationsDeviceRegistrationActorFactory>().As<IPushNotificationsDeviceRegistrationActorFactory>();
        }
    }
}