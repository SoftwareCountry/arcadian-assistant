namespace Arcadia.Assistant.DI
{
    using Microsoft.Extensions.Configuration;

    using Autofac;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Configuration;
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
            var vacationsEmailLoaderConfiguration = this.configuration
                .GetSection("VacationsEmailLoader")
                .Get<VacationsEmailLoaderConfiguration>();
            builder.RegisterInstance(vacationsEmailLoaderConfiguration).AsSelf();

            var accountingReminderConfiguration = this.configuration
                .GetSection("AccountingReminder")
                .Get<AccountingReminderConfiguration>();
            builder.RegisterInstance(accountingReminderConfiguration).AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();
            builder.RegisterType<ArcadiaVacationCreditRegistry>().As<VacationsCreditRegistry>();
            builder.RegisterType<EmployeesQueryExecutor>().AsSelf();
            builder.RegisterType<VacationsSyncExecutor>().AsSelf();
            builder.RegisterType<VacationsEmailLoader>().AsSelf();
            builder.RegisterType<CspCalendarEventsApprovalsChecker>().As<CalendarEventsApprovalsChecker>();
            builder.RegisterType<VacationAccountingReadyReminderActor>().AsSelf();
            builder.RegisterType<CspVacationsRegistry>().AsSelf();

            builder.RegisterType<CspEmployeeVacationsRegistryPropsFactory>().As<IEmployeeVacationsRegistryPropsFactory>();
        }
    }
}