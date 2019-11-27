namespace Arcadia.Assistant.MobileBuild.Contracts
{
    using Autofac;

    public class MobileBuildModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MobileBuildActorFactory>().As<IMobileBuildActorFactory>();
        }
    }
}