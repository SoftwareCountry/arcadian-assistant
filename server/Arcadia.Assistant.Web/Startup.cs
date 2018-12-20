namespace Arcadia.Assistant.Web
{
    using System.Collections.Generic;

    using Autofac;
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Swashbuckle.AspNetCore.Swagger;
    using ZNetCS.AspNetCore.Authentication.Basic;

    using Akka.Actor;
    using Akka.Configuration;
    using Akka.DI.AutoFac;
    using Akka.DI.Core;

    using Arcadia.Assistant.Configuration;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Authorization.Handlers;
    using Arcadia.Assistant.Web.Authorization.Requirements;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Download;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Health;
    using Arcadia.Assistant.Web.Infrastructure;
    using Arcadia.Assistant.Web.PushNotifications;
    using Arcadia.Assistant.Web.UserPreferences;
    using Arcadia.Assistant.Web.Users;

    public class Startup
    {
        public Startup(IHostingEnvironment environment)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddHoconContent("akka.conf", "Akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                configBuilder.AddUserSecrets<Startup>();
            }

            this.AppSettings = configBuilder.Build().Get<AppSettings>();

            var systemName = this.AppSettings.Server.ActorSystemName;
            var config = ConfigurationFactory.ParseString(this.AppSettings.Akka);
            this.ActorSystem = ActorSystem.Create(systemName, config);
        }

        public AppSettings AppSettings { get; }

        public ActorSystem ActorSystem { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ReSharper disable once UnusedMember.Global
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddHttpClient();

            var appSettings = this.AppSettings;

            services.AddSwaggerGen(
                c =>
                    {
                        c.SwaggerDoc("v1", new Info() { Title = "Arcadian-Assistant API", Version = "v1" });

                        var scheme = new OAuth2Scheme();
                        scheme.Flow = "implicit";

                        scheme.AuthorizationUrl = appSettings.Security.AuthorizationUrl;
                        scheme.TokenUrl = appSettings.Security.TokenUrl;

                        c.AddSecurityDefinition("oauth2", scheme);
                    });

            services.ConfigureSwaggerGen(
                x =>
                    {
                        x.DescribeAllEnumsAsStrings();
                        //x.CustomSchemaIds(t => t.FullName);
                    });

            services.AddTransient<AkkaTimeoutExceptionHandler>();

            services.AddScoped<AuthenticationEvents>();

            services
                .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(
                    jwtOptions =>
                    {
                        jwtOptions.Audience = appSettings.Security.ClientId;
                        jwtOptions.MetadataAddress = appSettings.Security.OpenIdConfigurationUrl;
                        jwtOptions.Events = new JwtEventsHandler();
                    })
                .AddBasicAuthentication(
                    basicOptions =>
                    {
                        basicOptions.Realm = this.AppSettings.ServiceEndpointsAuthentication.Realm;
                        basicOptions.EventsType = typeof(AuthenticationEvents);
                    });

            services.AddSingleton<ITelemetryInitializer, WebTelemetryInitializer>();
            services
                .AddAuthorization(options =>
                {
                    options.AddPolicy(Policies.UserIsEmployee, policy => policy.Requirements.Add(new UserIsEmployeeRequirement()));

                    options.AddPolicy(Policies.ServiceEndpoint, policy =>
                    {
                        policy.AddAuthenticationSchemes(BasicAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme);
                        policy.RequireAuthenticatedUser();
                    });
                });
        }

        // ReSharper disable once UnusedMember.Global
        public void ConfigureContainer(ContainerBuilder builder)
        {
            var appSettings = this.AppSettings;
            var systemName = appSettings.Server.ActorSystemName;
            var host = appSettings.Server.Host;
            var port = appSettings.Server.Port;

            var pathsBuilder = new ActorPathsBuilder(systemName, host, port);

            builder.RegisterInstance(appSettings).As<ITimeoutSettings>();
            builder.RegisterInstance(appSettings.Security).As<ISecuritySettings>();
            builder.RegisterInstance(appSettings.ServiceEndpointsAuthentication).As<IServiceEndpointsAuthenticationSettings>();
            builder.RegisterInstance(appSettings.DownloadApplication).As<IDownloadApplicationSettings>();
            builder.RegisterInstance(this.ActorSystem).As<IActorRefFactory>();
            builder.RegisterInstance(pathsBuilder).AsSelf();

            builder.RegisterType<DownloadActor>().AsSelf();

            builder.RegisterType<EmployeesRegistry>().As<IEmployeesRegistry>();
            builder.RegisterType<UserEmployeeSearch>().As<IUserEmployeeSearch>();
            builder.RegisterType<HealthService>().As<IHealthService>();
            builder.RegisterType<UserPreferencesService>().As<IUserPreferencesService>();
            builder.RegisterType<PushNotificationsService>().As<IPushNotificationsService>();

            builder.RegisterType<UserIsEmployeeHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();
            builder.RegisterType<EmployeePermissionsHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();
            builder.RegisterType<EditCalendarEventsPermissionHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();
            builder.RegisterType<CanApproveOnBehalfPermissionHandler>().As<IAuthorizationHandler>().InstancePerLifetimeScope();

            builder.RegisterType<PermissionsLoader>().As<IPermissionsLoader>().InstancePerLifetimeScope();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ISecuritySettings securitySettings,
            ILifetimeScope container)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // ReSharper disable once ObjectCreationAsStatement
            new AutoFacDependencyResolver(container, this.ActorSystem);

            this.ActorSystem.ActorOf(
                this.ActorSystem.DI().Props<DownloadActor>(),
                WellKnownActorPaths.DownloadApplicationBuilds);

            app.UseAkkaTimeoutExceptionHandler();
            app.UseAuthentication();

            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                    {
                        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Arcadian-Assistant API");
                        c.ShowJsonEditor();
                        c.ShowRequestHeaders();
                        c.ConfigureOAuth2(
                            securitySettings.ClientId,
                            null,
                            securitySettings.SwaggerRedirectUri,
                            "ArcadiaAssistant",
                            additionalQueryStringParameters: new Dictionary<string, string>() { { "resource", securitySettings.ClientId } }
                            );
                    });

            app.UseMvc();
        }
    }
}
