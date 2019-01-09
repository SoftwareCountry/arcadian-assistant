namespace Arcadia.Assistant.DI
{
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.InboxEmail;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.UserPreferences;

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
            builder.RegisterType<EmployeesQueryExecutor>().AsSelf();
            builder.RegisterType<VacationsSyncExecutor>().AsSelf();
            builder.RegisterType<VacationsEmailLoader>().AsSelf();

            builder.RegisterType<CspCalendarEventsApprovalsChecker>().As<CalendarEventsApprovalsChecker>();

            builder.RegisterType<UserPreferencesActor>().AsSelf();
            builder.RegisterType<InboxEmailActor>().AsSelf();
        }
    }
}