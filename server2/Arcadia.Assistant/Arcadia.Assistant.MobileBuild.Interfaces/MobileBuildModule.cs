namespace Arcadia.Assistant.MobileBuild.Contracts
{
    using Arcadia.Assistant.MobileBuild.Contracts.Interfaces;
    using Autofac;

    public class MobileBuildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MobileBuildActorFactory>().As<IMobileBuildActorFactory>();
        }
    }
}