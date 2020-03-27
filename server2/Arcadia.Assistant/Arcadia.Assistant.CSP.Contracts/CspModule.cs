namespace Arcadia.Assistant.CSP.Contracts
{
    using System;

    using Autofac;
    using Autofac.Core;

    using Microsoft.ServiceFabric.Services.Remoting.Client;

    public class CspModule : Module
    {
        public CspModule(string connectionString)
        {

        }

        protected override void Load(ContainerBuilder builder)
        {
            var options = "";
            builder.Register(x => new ArcadiaCspContext(options)).InstancePerDependency();
            builder.Register(x => new CspConfiguration()
            {
                CompanyId = 154, //TODO: config file
                HeadDepartmentAbbreviation = "GMG",
                UserIdentityDomain = "arcadia.spb.ru"
            }).SingleInstance();
            builder.RegisterType<CspEmployeeQuery>().InstancePerDependency();
            builder.RegisterType<CspDepartmentsQuery>().InstancePerDependency();
            /*
            builder.Register(x => x
                .Resolve<IServiceProxyFactory>()
                .CreateServiceProxy<IEmailNotifications>(
                    new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.EmailNotifications")));
                    */
        }
    }
}