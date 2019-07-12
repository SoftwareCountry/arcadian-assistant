namespace Arcadia.Assistant.VacationsCredit
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class VacationsCredit : StatelessService, IVacationsCredit
    {
        private readonly InboxConfiguration configuration;
        private readonly Func<Owned<IVacationsDaysLoader>> loaderFactory;

        private Dictionary<string, double> EmailToVacationDaysCount { get; set; }

        public VacationsCredit(StatelessServiceContext context, InboxConfiguration configuration, Func<Owned<IVacationsDaysLoader>> loaderFactory)
            : base(context)
        {
            this.configuration = configuration;
            this.loaderFactory = loaderFactory;
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

                ServiceEventSource.Current.ServiceMessage(this.Context, "Checking inbox");

                using (var loader = this.loaderFactory())
                {
                    try
                    {
                        this.EmailToVacationDaysCount = await loader.Value.GetEmailsToDaysMappingAsync(cancellationToken);
                    }
                    catch (TaskCanceledException) { }
                    catch (Exception e)
                    {
                        ServiceEventSource.Current.ServiceMessage(this.Context, "Error occurred: {0}", e.Message);
                    }
                }

                await Task.Delay(this.configuration.RefreshInterval, cancellationToken);
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