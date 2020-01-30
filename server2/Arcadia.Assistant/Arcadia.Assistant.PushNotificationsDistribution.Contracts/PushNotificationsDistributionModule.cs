namespace Arcadia.Assistant.PushNotificationsDistributor.Contracts
{
    using Autofac;

    public class PushNotificationsDistributionModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<PushNotificationsDistributionActorFactory>().As<IPushNotificationsDistributionActorFactory>();
        }
    }
}