namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP.Model;

    using Employees.Contracts;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class SickLeaves : StatelessService, ISickLeaves
    {
        private readonly Func<Owned<ArcadiaCspContext>> dbFactory;
        private readonly Func<Owned<SickLeaveCreationStep>> creationStepsFactory;
        private readonly SickLeaveModelConverter modelConverter = new SickLeaveModelConverter();

        public SickLeaves(StatelessServiceContext context, 
            Func<Owned<ArcadiaCspContext>> dbFactory,
            Func<Owned<SickLeaveCreationStep>> creationStepsFactory)
            : base(context)
        {
            this.dbFactory = dbFactory;
            this.creationStepsFactory = creationStepsFactory;
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

        public async Task<SickLeaveDescription[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using (var db = this.dbFactory())
            {
                var sickLeaves = await db.Value
                    .SickLeaves
                    .Where(x => x.EmployeeId == employeeId.Value)
                    .Select(this.modelConverter.ToDescription)
                    .ToArrayAsync(cancellationToken);

                return sickLeaves;
            }
        }

        public async Task<SickLeaveDescription> GetCalendarEventAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken)
        {
            using (var db = this.dbFactory())
            {
                var sickLeave = await db.Value
                    .SickLeaves
                    .Where(x => x.Id == eventId && x.EmployeeId == employeeId.Value)
                    .Select(this.modelConverter.ToDescription)
                    .FirstOrDefaultAsync(cancellationToken);

                return sickLeave;
            }
        }

        public async Task<SickLeaveDescription> CreateSickLeaveAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate)
        {
            using (var step = this.creationStepsFactory())
            {
                return await step.Value.Invoke(employeeId, startDate, endDate);
            }
        }

        public Task ProlongSickLeaveAsync(EmployeeId employeeId, int eventId, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        public Task CancelSickLeaveAsync(EmployeeId employeeId, int eventId)
        {
            throw new NotImplementedException();
        }
    }
}