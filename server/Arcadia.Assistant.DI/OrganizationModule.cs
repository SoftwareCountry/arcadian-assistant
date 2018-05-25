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
            builder.RegisterType<OrganizationActor>().AsSelf();

            builder.RegisterType<DepartmentsStorage>().AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();
        }
    }
}