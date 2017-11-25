namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;

    using Autofac;

    public class OrganizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployeesActor>().AsSelf();
            builder.RegisterType<DemographicsLoaderActor>().AsSelf();

            builder.RegisterType<AllEmployeesQuery>().As<IAllEmployeesQuery>();
        }
    }
}