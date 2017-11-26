namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;

    using Autofac;

    using AllEmployeesQuery = Organization.Abstractions.AllEmployeesQuery;

    public class OrganizationModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployeesActor>().AsSelf();

            builder.RegisterType<CSP.AllEmployeesQueryImpl>().As<AllEmployeesQuery>();
            builder.RegisterType<CSP.EmployeeInfoQueryImpl>().As<EmployeeInfoQuery>();
        }
    }
}