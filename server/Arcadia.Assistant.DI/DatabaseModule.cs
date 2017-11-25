namespace Arcadia.Assistant.DI
{
    using Arcadia.Assistant.CSP.Model;

    using Autofac;

    using Microsoft.EntityFrameworkCore;

    public class DatabaseModule : Module
    {
        private readonly string cspConnectionString;

        public DatabaseModule(string cspConnectionString)
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
        }
    }
}