namespace Arcadia.Assistant.Web
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Configuration;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ServiceFabric.AspNetCore.Configuration;
    using Microsoft.ServiceFabric.Services.Client;
    using Microsoft.ServiceFabric.Services.Remoting.Client;

    using NSwag;
    using NSwag.AspNetCore;
    using NSwag.SwaggerGeneration.Processors.Security;

    using Organization.Contracts;

    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .AddServiceFabricConfiguration()
                .AddEnvironmentVariables();
            this.AppSettings = builder.Build().Get<AppSettings>();
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

            services.AddSingleton(this.AppSettings);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddOpenApiDocument((document, x) =>
            {
                var settings = x.GetService<AppSettings>().Security;
                document.AddSecurity("bearer", new List<string>(), new SwaggerSecurityScheme()
                {
                    Type = SwaggerSecuritySchemeType.OAuth2,
                    Description = "Oauth",
                    Flow = SwaggerOAuth2Flow.Implicit,
                    Flows = new OpenApiOAuthFlows()
                    {
                        Implicit = new OpenApiOAuthFlow()
                        {
                            AuthorizationUrl = settings.AuthorizationUrl,
                            TokenUrl = settings.TokenUrl
                        }
                    }
                });

                document.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor("bearer"));
            });

            services.AddSingleton<IServiceProxyFactory>(new ServiceProxyFactory());
            services.AddTransient(x => 
                x.GetService<IServiceProxyFactory>()
                    .CreateServiceProxy<IOrganization>(new Uri("fabric:/Arcadia.Assistant.SF/Arcadia.Assistant.Organization"), new ServicePartitionKey(0)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, AppSettings appSettings)
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

            app.UseSwagger();
            app.UseSwaggerUi3((settings) =>
            {
                settings.OAuth2Client = new OAuth2ClientSettings()
                {
                    ClientId = appSettings.Security.ClientId
                };
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}