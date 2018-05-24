namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;

    using Autofac;
    using Calendar.SickLeave;
    using Microsoft.Extensions.Configuration;

    public class OrganizationModule : Module
    {
        private readonly IConfigurationSection mailConfig;

        public OrganizationModule(IConfigurationSection mailConfig)
        {
            this.mailConfig = mailConfig;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmployeesActor>().AsSelf();
            builder.RegisterType<OrganizationActor>().AsSelf();

            builder.RegisterType<DepartmentsStorage>().AsSelf();

            builder.RegisterType<CspDepartmentsStorage>().As<DepartmentsStorage>();
            builder.RegisterType<CspEmployeesInfoStorage>().As<EmployeesInfoStorage>();

            builder.Register(x => new SendEmailSickLeaveActor(mailConfig));
        }
    }
}