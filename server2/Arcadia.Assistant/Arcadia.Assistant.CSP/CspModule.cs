namespace Arcadia.Assistant.CSP
{
    using Autofac;

    using Microsoft.EntityFrameworkCore;

    using Model;

    public class CspModule : Module
    {
        private readonly string cspConnectionString;

        public CspModule(string cspConnectionString)
        {
            this.cspConnectionString = cspConnectionString;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ArcadiaCspContext>()
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlServer(this.cspConnectionString);

            var options = optionsBuilder.Options;

            builder.Register(x => new ArcadiaCspContext(options)).InstancePerDependency();
            builder.Register(x => new CspConfiguration() { CompanyId = 154 }).SingleInstance();
            builder.RegisterType<CspEmployeeQuery>().InstancePerDependency();
        }
    }
}