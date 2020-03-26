namespace Arcadia.Assistant.Avatars.Manager
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP.Contracts;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Manager : StatelessService
    {
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;
        private readonly IAvatars avatars;
        private readonly ILogger logger;

        public Manager(StatelessServiceContext context, Func<Owned<CspEmployeeQuery>> employeeQuery, IAvatars avatars, ILogger<Manager> logger)
            : base(context)
        {
            this.employeeQuery = employeeQuery;
            this.avatars = avatars;
            this.logger = logger;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                this.logger.LogInformation("Start updating avatars...");

                using (var query = this.employeeQuery())
                {
                    try
                    {
                        var employees = query.Value.Get().Select(x => new { x.Id, x.Image }).ToList();

                        foreach (var employee in employees)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            var actor = this.avatars.Get(new EmployeeId(employee.Id));
                            await actor.SetSource(employee.Image);
                        }
                    }
                    catch (Exception e)
                    {
                        this.logger.LogError(e, e.Message);
                    }
                }

                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }
    }
}