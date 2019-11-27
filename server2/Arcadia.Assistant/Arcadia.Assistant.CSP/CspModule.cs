namespace Arcadia.Assistant.CSP
{
    using Autofac;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Logging.Debug;

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
                .UseLoggerFactory(new LoggerFactory(new[] { new DebugLoggerProvider(), }))
                .UseSqlServer(this.cspConnectionString);

            var options = optionsBuilder.Options;

            builder.Register(x => new ArcadiaCspContext(options)).InstancePerDependency();
            builder.Register(x => new CspConfiguration()
            {
                CompanyId = 154, //TODO: config file
                HeadDepartmentAbbreviation = "GMG",
                UserIdentityDomain = "arcadia.spb.ru"
            }).SingleInstance();

            builder.RegisterType<CspEmployeeQuery>().InstancePerDependency();
            builder.RegisterType<CspDepartmentsQuery>().InstancePerDependency();
        }
    }
}