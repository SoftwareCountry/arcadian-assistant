namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.CSP.Contracts;
    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP.Contracts.Models;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Permissions.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class SickLeaves : StatelessService, ISickLeaves
    {
        private readonly Func<Owned<ArcadiaCspContext>> dbFactory;
        private readonly Func<Owned<SickLeaveCreationStep>> creationStepsFactory;
        private readonly Func<Owned<SickLeaveProlongationStep>> prolongationStepsFactory;
        private readonly Func<Owned<SickLeaveCancellationStep>> cancellationStepsFactory;

        private readonly SickLeaveModelConverter modelConverter = new SickLeaveModelConverter();
        private readonly ILogger logger;

        public SickLeaves(
            StatelessServiceContext context,
            Func<Owned<ArcadiaCspContext>> dbFactory,
            Func<Owned<SickLeaveCreationStep>> creationStepsFactory,
            Func<Owned<SickLeaveProlongationStep>> prolongationStepsFactory,
            Func<Owned<SickLeaveCancellationStep>> cancellationStepsFactory,
            ILogger<SickLeaves> logger)
            : base(context)
        {
            this.dbFactory = dbFactory;
            this.creationStepsFactory = creationStepsFactory;
            this.prolongationStepsFactory = prolongationStepsFactory;
            this.cancellationStepsFactory = cancellationStepsFactory;
            this.logger = logger;
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

        public SickLeaveDescription[] GetCalendarEvents(
            EmployeeId employeeId)
        {
            using var db = this.dbFactory();
            var sickLeaves = this.GetSickLeaves(db.Value)
                .Where(x => x.EmployeeId == employeeId.Value)
                .ToArray();

            return sickLeaves.Select(this.modelConverter.GetDescription).ToArray();
        }

        public Dictionary<EmployeeId, SickLeaveDescription[]> GetCalendarEventsByEmployeeMap(
            EmployeeId[] employeeIds)
        {
            using var db = this.dbFactory();
            var sickLeaves = this.GetSickLeaves(db.Value)
                .Where(x => employeeIds.Any(id => x.EmployeeId == id.Value))
                .ToArray();

            return sickLeaves
                .Select(this.modelConverter.GetDescription)
                .GroupBy(x => x.EmployeeId).ToDictionary(x => x.Key, x => x.ToArray());
        }

        public SickLeaveDescription GetCalendarEvent(
            EmployeeId employeeId, int eventId)
        {
            using var db = this.dbFactory();
            var sickLeave = this.GetSickLeaves(db.Value)
                .FirstOrDefault(x => x.Id == eventId && x.EmployeeId == employeeId.Value);

            return this.modelConverter.GetDescription(sickLeave);
        }

        public async Task<SickLeaveDescription> CreateSickLeaveAsync(
            EmployeeId employeeId, DateTime startDate, DateTime endDate, UserIdentity userIdentity)
        {
            using var step = this.creationStepsFactory();
            return await step.Value.InvokeAsync(employeeId, startDate, endDate, userIdentity);
        }

        public async Task ProlongSickLeaveAsync(
            EmployeeId employeeId, int eventId, DateTime endDate, UserIdentity userIdentity)
        {
            using var step = this.prolongationStepsFactory();
            await step.Value.InvokeAsync(employeeId, eventId, endDate, userIdentity);
        }

        public async Task CancelSickLeaveAsync(EmployeeId employeeId, int eventId, UserIdentity cancelledBy)
        {
            using var step = this.cancellationStepsFactory();
            await step.Value.InvokeAsync(employeeId, eventId, cancelledBy);
        }

        private IQueryable<SickLeave> GetSickLeaves(ArcadiaCspContext context)
        {
            return new List<SickLeave>().AsQueryable();
            /*
            return context.SickLeaves
                .Include(x => x.SickLeaveCancellations)
                .Include(x => x.SickLeaveCompletes);
                */
        }
    }
}