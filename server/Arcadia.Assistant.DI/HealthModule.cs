namespace Arcadia.Assistant.DI
{
    using Autofac;
    using CSP;
    using Health.Abstractions;

    public class HealthModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ArcadiaHealthChecker>().As<HealthChecker>();
        }
    }
}