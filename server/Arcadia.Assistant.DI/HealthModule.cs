namespace Arcadia.Assistant.DI
{
    using Autofac;
    using Health;

    public class HealthModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<HealthActor>().AsSelf();
        }
    }
}