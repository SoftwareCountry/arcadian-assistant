using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
// ReSharper disable UnusedMember.Global

namespace Arcadia.Assistant.Web
{
    using Akka.Actor;
    using Akka.Configuration;

    using Arcadia.Assistant.Configuration;
    using Arcadia.Assistant.Server.Interop;

    using Autofac;

    using Swashbuckle.AspNetCore.Swagger;

    public class Startup
    {
        private readonly IHostingEnvironment environment;

        public Startup(IHostingEnvironment environment)
        {
            this.environment = environment;
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional:true)
                .AddHoconContent("akka.conf", "akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                configBuilder.AddUserSecrets<Startup>();
            }

            this.Configuration = configBuilder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var serverConfig = this.Configuration.GetSection("Server");

            var systemName = serverConfig["ActorSystemName"];
            var host = serverConfig["Host"];
            var port = serverConfig.GetValue<int>("Port");

            var config = ConfigurationFactory.ParseString(this.Configuration["akka"]);

            var actorSystem = ActorSystem.Create(systemName, config);

            var pathsBuilder = new ActorPathsBuilder(systemName, host, port);

            services.AddSingleton<IActorRefFactory>(actorSystem);
            services.AddSingleton(pathsBuilder);

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new Info() { Title = "Arcadian-Assistant API", Version = "v1" }); });
            services.ConfigureSwaggerGen(
                x =>
                    {
                        x.DescribeAllEnumsAsStrings();
                        //x.CustomSchemaIds(t => t.FullName);
                    });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arcadian-Assistant API");
                        c.ShowJsonEditor();
                        c.ShowRequestHeaders();
                    });

            app.UseMvc();
        }
    }
}
