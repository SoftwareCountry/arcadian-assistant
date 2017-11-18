using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Arcadia.Assistant.Web
{
    using Akka.Actor;
    using Akka.Configuration;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            var config = ConfigurationFactory.ParseString(
                @"
                akka {
                    actor {
                        provider: remote
                    }

                remote {
                    dot-netty.tcp {
                        port: 0
                    }
                }
            ");

            var actorSystem = ActorSystem.Create("arcadia-assistant", config);
            services.AddSingleton(actorSystem);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
