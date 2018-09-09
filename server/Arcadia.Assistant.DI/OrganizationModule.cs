namespace Arcadia.Assistant.DI
{
    using System;
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.CSP;
    using Arcadia.Assistant.CSP.Vacations;
    using Arcadia.Assistant.Organization;
    using Arcadia.Assistant.Organization.Abstractions;

    using Autofac;
    using Configuration.Configuration;

    using Microsoft.EntityFrameworkCore;

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