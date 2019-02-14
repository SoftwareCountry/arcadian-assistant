namespace Arcadia.Assistant.DI
{
    using Microsoft.Extensions.Configuration;

    using Autofac;

    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.Organization.Abstractions;

    public class ArcadiaCspModule : Module
    {
        private readonly IConfigurationRoot configuration;

        public ArcadiaCspModule(IConfigurationRoot configuration)
        {
            this.configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var settings = this.configuration
                .GetSection("VacationsEmailLoader")
                .Get<VacationsEmailLoaderConfiguration>();
            builder.RegisterInstance(settings).AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();
            builder.RegisterType<ArcadiaVacationRegistry>().As<VacationsRegistry>();
            builder.RegisterType<EmployeesQueryExecutor>().AsSelf();
            builder.RegisterType<VacationsSyncExecutor>().AsSelf();
            builder.RegisterType<VacationsEmailLoader>().AsSelf();
            builder.RegisterType<CspCalendarEventsApprovalsChecker>().As<CalendarEventsApprovalsChecker>();
        }
    }
}