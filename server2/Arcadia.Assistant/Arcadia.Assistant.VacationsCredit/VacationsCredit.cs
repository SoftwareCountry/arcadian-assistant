namespace Arcadia.Assistant.VacationsCredit
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class VacationsCredit : StatelessService, IVacationsCredit
    {
        private Dictionary<string, double> EmailToVacationDaysCount { get; set; }

        public VacationsCredit(StatelessServiceContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
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

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working");

                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }

        public Task<int?> GetVacationDaysLeftAsync(string email, CancellationToken cancellationToken)
        {
            int? result = null;
            if (this.EmailToVacationDaysCount.TryGetValue(email, out var value))
            {
                result = (int)value;
            }

            return Task.FromResult(result);
        }
    }
}