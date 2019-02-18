namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.Calendar.Abstractions;
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
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
            builder.Register(ctx => new OrganizationActor(
                this.refreshInformation,
                ctx.Resolve<IEmployeeVacationsRegistryPropsFactory>()));

            builder.RegisterType<DepartmentsStorage>().AsSelf();

            //builder.RegisterType<PersistentEmployeeVacationsRegistryPropsFactory>()
            //    .As<IEmployeeVacationsRegistryPropsFactory>();

            builder.RegisterType<UserPreferencesActor>().AsSelf();
            builder.RegisterType<InboxEmailActor>().AsSelf();
        }
    }
}