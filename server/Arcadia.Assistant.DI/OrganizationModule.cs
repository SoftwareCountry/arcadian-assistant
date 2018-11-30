namespace Arcadia.Assistant.DI
{
    using System;
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.UserPreferences;

    public class OrganizationModule : Module
    {
        private readonly IRefreshInformation refreshInformation;
        private readonly TimeSpan timeoutSetting;

        public OrganizationModule(IRefreshInformation refreshInformation, TimeSpan timeoutSetting)
        {
            this.refreshInformation = refreshInformation;
            this.timeoutSetting = timeoutSetting;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployeesActor>().AsSelf();
            builder.Register(ctx => new OrganizationActor(this.refreshInformation, this.timeoutSetting));

            builder.RegisterType<DepartmentsStorage>().AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();

            builder.RegisterType<ArcadiaVacationRegistry>().As<VacationsRegistry>();
            builder.RegisterType<VacationsQueryExecutor>().AsSelf();
            builder.RegisterType<CspVacationApprovalsChecker>().As<VacationApprovalsChecker>();

            builder.RegisterType<UserPreferencesActor>().AsSelf();
        }
    }
}