namespace Arcadia.Assistant.Web
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Configuration;

    using Arcadia.Assistant.Configuration;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;

    using Autofac;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class Startup
    {
        public Startup(IHostingEnvironment environment)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional:true)
                .AddHoconContent("akka.conf", "Akka", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            if (environment.IsDevelopment())
            {
                configBuilder.AddUserSecrets<Startup>();
            }

            this.AppSettings = configBuilder.Build().Get<AppSettings>();
        }

        public AppSettings AppSettings { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ReSharper disable once UnusedMember.Global
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            var appSettings = this.AppSettings;

            var systemName = appSettings.Server.ActorSystemName;
            var host = appSettings.Server.Host;
            var port = appSettings.Server.Port;

            var config = ConfigurationFactory.ParseString(appSettings.Akka);

            var actorSystem = ActorSystem.Create(systemName, config);

            var pathsBuilder = new ActorPathsBuilder(systemName, host, port);

            services.AddSingleton<ITimeoutSettings>(appSettings);
            services.AddSingleton<ISecuritySettings>(appSettings.Security);
            services.AddSingleton<IActorRefFactory>(actorSystem);
            services.AddSingleton(pathsBuilder);

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

            services
                .AddAuthentication(options => { options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
                .AddJwtBearer(
                    jwtOptions =>
                        {
                            jwtOptions.Audience = appSettings.Security.ClientId;
                            jwtOptions.MetadataAddress = appSettings.Security.OpenIdConfigurationUrl;
                        });
        }

        // ReSharper disable once UnusedMember.Global
        public void ConfigureContainer(ContainerBuilder builder)
        {
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // ReSharper disable once UnusedMember.Global
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ISecuritySettings securitySettings)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
