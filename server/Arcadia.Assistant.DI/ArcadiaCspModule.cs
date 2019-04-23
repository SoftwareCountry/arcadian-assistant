namespace Arcadia.Assistant.DI
{
    using System.Linq;
    using System.Net.Http;

    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Configuration;
    using Arcadia.Assistant.CSP.Sharepoint;
    using Arcadia.Assistant.CSP.SickLeaves;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts;
    using Arcadia.Assistant.Organization.Abstractions;

    using Autofac;

    using Microsoft.Extensions.Configuration;

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

            var vacationsAccountingReminderConfiguration = this.configuration
                .GetSection("VacationsAccountingReminder")
                .Get<VacationsAccountingReminderConfiguration>();
            builder.RegisterInstance(vacationsAccountingReminderConfiguration).AsSelf();

            var sickLeavesAccountingReminderConfiguration = this.configuration
                .GetSection("SickLeavesAccountingReminder")
                .Get<SickLeavesAccountingReminderConfiguration>();
            builder.RegisterInstance(sickLeavesAccountingReminderConfiguration).AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();
            builder.RegisterType<ArcadiaVacationCreditRegistry>().As<VacationsCreditRegistry>();
            builder.RegisterType<EmployeesQueryExecutor>().AsSelf();
            builder.RegisterType<VacationsSyncExecutor>().AsSelf();
            builder.RegisterType<SickLeavesSyncExecutor>().AsSelf();
            builder.RegisterType<VacationsEmailLoader>().AsSelf();
            builder.RegisterType<CspCalendarEventsApprovalsChecker>().As<CalendarEventsApprovalsChecker>();
            builder.RegisterType<VacationAccountingReadyReminderActor>().AsSelf();
            builder.RegisterType<SickLeaveEndingReminderActor>().AsSelf();
            builder.RegisterType<CspVacationsRegistry>().AsSelf();
            builder.RegisterType<CspSickLeavesRegistry>().AsSelf();
            builder.RegisterType<CspCalendarEventIdParser>().AsSelf();
            builder.RegisterType<SharepointStorageActor>().AsSelf();

            builder.RegisterType<CspEmployeeVacationsRegistryPropsFactory>().As<IEmployeeVacationsRegistryPropsFactory>();
            builder.RegisterType<CspEmployeeSickLeavesRegistryPropsFactory>().As<IEmployeeSickLeavesRegistryPropsFactory>();

            this.RegisterSharepoint(builder);
        }

        private void RegisterSharepoint(ContainerBuilder builder)
        {
            var sharepointConfiguration = this.configuration
                .GetSection("Sharepoint")
                .Get<SharepointSettings>();

            builder
                .RegisterInstance(new SharepointOnlineConfiguration
                {
                    ServerUrl = sharepointConfiguration.ServerUrl,
                    ClientId = sharepointConfiguration.ClientId,
                    ClientSecret = sharepointConfiguration.ClientSecret
                })
                .As<ISharepointOnlineConfiguration>();

            builder
                .Register(ctx =>
                {
                    if (sharepointConfiguration.CalendarEventIdField == null)
                    {
                        return new SharepointFieldsMapper();
                    }

                    var mapping = SharepointFieldsMapper.DefaultMapping
                        .Union(new[]
                        {
                            SharepointFieldsMapper.CreateMapping(x => x.CalendarEventId, sharepointConfiguration.CalendarEventIdField)
                        });
                    return new SharepointFieldsMapper(mapping.ToArray());
                })
                .As<ISharepointFieldsMapper>();

            builder.RegisterType<SharepointRequestExecutor>().As<ISharepointRequestExecutor>();
            builder.RegisterType<SharepointAuthTokenService>().As<ISharepointAuthTokenService>();
            builder.RegisterType<SharepointConditionsCompiler>().As<ISharepointConditionsCompiler>();
            builder.RegisterType<SharepointStorage>().As<IExternalStorage>();
        }
    }
}