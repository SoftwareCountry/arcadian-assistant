namespace Arcadia.Assistant.SickLeaves
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
    public class SickLeaves : StatelessService, ISickLeaves
    {
        public SickLeaves(StatelessServiceContext context)
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
                //TODO: Sharepoint sync
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }

        public Task<object[]> GetCalendarEventsAsync(string employeeId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<object> GetCalendarEventAsync(string employeeId, int eventId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task CreateSickLeaveAsync(string employeeId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task ProlongSickLeaveAsync(string employeeId, int eventId, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task CancelSickLeaveAsync(string employeeId, int eventId)
        {
            throw new NotImplementedException();
        }
    }
}