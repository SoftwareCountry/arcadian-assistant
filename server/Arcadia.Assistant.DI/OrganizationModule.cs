namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;

    using Autofac;

    using AllEmployeesQuery = Arcadia.Assistant.Organization.Abstractions.AllEmployeesQuery;

    public class OrganizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployeesActor>().AsSelf();
            builder.RegisterType<DemographicsLoaderActor>().AsSelf();

            builder.RegisterType<CSP.AllEmployeesQueryImp>().As<AllEmployeesQuery>();
        }
    }
}