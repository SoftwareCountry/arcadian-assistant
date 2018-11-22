namespace Arcadia.Assistant.DI
{
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;

    public class OrganizationModule : Module
    {
        private readonly IRefreshInformation refreshInformation;

        public OrganizationModule(IRefreshInformation refreshInformation)
        {
            this.refreshInformation = refreshInformation;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployeesActor>().AsSelf();
            builder.Register(ctx => new OrganizationActor(this.refreshInformation));

            builder.RegisterType<DepartmentsStorage>().AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();

            builder.RegisterType<ArcadiaVacationRegistry>().As<VacationsRegistry>();
            builder.RegisterType<VacationsQueryExecutor>().AsSelf();
        }
    }
}