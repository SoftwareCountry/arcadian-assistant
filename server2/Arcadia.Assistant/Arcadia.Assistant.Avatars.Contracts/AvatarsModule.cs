namespace Arcadia.Assistant.Avatars.Contracts
{
    using Autofac;

    public class AvatarsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvatarsActorFactory>().As<IAvatars>();
        }
    }
}