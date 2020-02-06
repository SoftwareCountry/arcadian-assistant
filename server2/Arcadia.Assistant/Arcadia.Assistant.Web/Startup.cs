﻿namespace Arcadia.Assistant.Web
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Text.Json.Serialization;

    using Autofac;

    using Avatars.Contracts;

    using Configuration;

    using Employees.Contracts;

    using Logging;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.AspNetCore.Configuration;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using MobileBuild.Contracts;

    using NSwag;
    using NSwag.AspNetCore;
    using NSwag.Generation.Processors.Security;

    using Organization.Contracts;

    using PendingActions.Contracts;

    using Permissions.Contracts;

    using SickLeaves.Contracts;

    using UserPreferences.Contracts;

    using Vacations.Contracts;

    using VacationsCredit.Contracts;

    using WorkHoursCredit.Contracts;

    public class Startup
    {
        private readonly StatelessServiceContext serviceContext;

        public Startup(IWebHostEnvironment env, StatelessServiceContext serviceContext)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddServiceFabricConfiguration()
                .AddEnvironmentVariables();
            this.AppSettings = builder.Build().Get<AppSettings>();
            this.serviceContext = serviceContext;
        }

        public AppSettings AppSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddControllersWithViews()
                .AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

            services.AddOpenApiDocument((document, x) =>
            {
                var settings = x.GetService<AppSettings>().Config.Security;
                document.AddSecurity("bearer", new List<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.OAuth2,
                    Description = "Oauth",
                    Flow = OpenApiOAuth2Flow.Implicit,
                    Flows = new OpenApiOAuthFlows
                    {
                        Implicit = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl = settings.AuthorizationUrl,
                            TokenUrl = settings.TokenUrl
                        }
                    }
                });

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(jwtOptions =>
                {
                    jwtOptions.Audience = this.AppSettings.Config.Security.ClientId;
                    jwtOptions.MetadataAddress = this.AppSettings.Config.Security.OpenIdConfigurationUrl;
                    jwtOptions.Events = new JwtBearerEvents();
                });
        }

        // ReSharper disable once UnusedMember.Global
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterInstance(this.AppSettings);
            builder.RegisterInstance(this.serviceContext).As<ServiceContext>();
            builder.RegisterInstance<IHelpSettings>(this.AppSettings.Config.Links);
            builder.RegisterInstance<ISslSettings>(this.AppSettings.Config.Ssl);
            builder.RegisterInstance<IServiceProxyFactory>(new ServiceProxyFactory());
            builder.RegisterInstance<IActorProxyFactory>(new ActorProxyFactory());
            builder.RegisterModule(new OrganizationModule());
            builder.RegisterModule(new EmployeesModule());
            builder.RegisterModule(new PermissionsModule());
            builder.RegisterModule(new AvatarsModule());
            builder.RegisterModule(new UsersPreferencesModule());
            builder.RegisterModule(new WorkHoursCreditModule());
            builder.RegisterModule(new VacationsCreditModule());
            builder.RegisterModule(new VacationsModule());
            builder.RegisterModule(new SickLeavesModule());
            builder.RegisterModule(new PendingActionsModule());
            builder.RegisterModule(new MobileBuildModule());
            builder.RegisterServiceLogging(new LoggerSettings(this.AppSettings.Config.Logging.ApplicationInsightsKey));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, AppSettings appSettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseOpenApi();
            app.UseSwaggerUi3(settings =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings
                {
                    ClientId = appSettings.Config.Security.ClientId
                };
            });

            app.UseAuthentication();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}