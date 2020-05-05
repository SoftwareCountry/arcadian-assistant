namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.Notifications.Contracts.Models;
    using Autofac.Features.OwnedInstances;

    using Configuration;

    using Contracts;

    using CSP.Model;

    using Employees.Contracts;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Notifications;

    using NotificationTemplates;

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

        private readonly ISickLeaveCreateNotificationConfiguration sickLeaveCreateNotificationConfiguration;
        private readonly ISickLeaveProlongNotificationConfiguration sickLeaveProlongNotificationConfiguration;
        private readonly ISickLeaveCancelNotificationConfiguration sickLeaveCancelNotificationConfiguration;
        private readonly SickLeaveChangeNotification notification;
        private readonly SickLeaveModelConverter modelConverter = new SickLeaveModelConverter();
        private readonly ILogger logger;

        public SickLeaves(
            StatelessServiceContext context,
            Func<Owned<ArcadiaCspContext>> dbFactory,
            Func<Owned<SickLeaveCreationStep>> creationStepsFactory,
            Func<Owned<SickLeaveProlongationStep>> prolongationStepsFactory,
            Func<Owned<SickLeaveCancellationStep>> cancellationStepsFactory,
            ISickLeaveCreateNotificationConfiguration sickLeaveCreateNotificationConfiguration,
            ISickLeaveProlongNotificationConfiguration sickLeaveProlongNotificationConfiguration,
            ISickLeaveCancelNotificationConfiguration sickLeaveCancelNotificationConfiguration,
            SickLeaveChangeNotification notification,
            ILogger<SickLeaves> logger)
            : base(context)
        {
            this.dbFactory = dbFactory;
            this.creationStepsFactory = creationStepsFactory;
            this.prolongationStepsFactory = prolongationStepsFactory;
            this.cancellationStepsFactory = cancellationStepsFactory;
            this.sickLeaveCreateNotificationConfiguration = sickLeaveCreateNotificationConfiguration;
            this.sickLeaveProlongNotificationConfiguration = sickLeaveProlongNotificationConfiguration;
            this.sickLeaveCancelNotificationConfiguration = sickLeaveCancelNotificationConfiguration;
            this.notification = notification;
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
                await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
            }
        }

        public async Task<SickLeaveDescription[]> GetCalendarEventsAsync(
            EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var db = this.dbFactory();
            var sickLeaves = await this.GetSickLeaves(db.Value)
                .Where(x => x.EmployeeId == employeeId.Value)
                .ToArrayAsync(cancellationToken);

            return sickLeaves.Select(this.modelConverter.GetDescription).ToArray();
        }

        public async Task<Dictionary<EmployeeId, SickLeaveDescription[]>> GetCalendarEventsByEmployeeMapAsync(
            EmployeeId[] employeeIds, CancellationToken cancellationToken)
        {
            using var db = this.dbFactory();
            var sickLeaves = await this.GetSickLeaves(db.Value)
                .Where(x => employeeIds.Any(id => x.EmployeeId == id.Value))
                .ToArrayAsync(cancellationToken);

            return sickLeaves
                .Select(this.modelConverter.GetDescription)
                .GroupBy(x => x.EmployeeId).ToDictionary(x => x.Key, x => x.ToArray());
        }

        public async Task<SickLeaveDescription?> GetCalendarEventAsync(
            EmployeeId employeeId, int eventId, CancellationToken cancellationToken)
        {
            using var db = this.dbFactory();
            var sickLeave = await this.GetSickLeaves(db.Value)
                .Where(x => x.Id == eventId && x.EmployeeId == employeeId.Value)
                .FirstOrDefaultAsync(cancellationToken);

            return this.modelConverter.GetDescription(sickLeave);
        }

        public async Task<SickLeaveDescription> CreateSickLeaveAsync(
            EmployeeId employeeId, DateTime startDate, DateTime endDate, UserIdentity userIdentity)
        {
            using var step = this.creationStepsFactory();
            var eventContext = await step.Value.InvokeAsync(employeeId, startDate, endDate, userIdentity);

            await this.notification.SendNotification(
                eventContext.SickLeaveId,
                employeeId,
                SickLeaveNotificationTemplate.SickLeaveCreated,
                sickLeaveCreateNotificationConfiguration,
                CancellationToken.None);
            return eventContext;
        }

        public async Task ProlongSickLeaveAsync(
            EmployeeId employeeId, int eventId, DateTime endDate, UserIdentity userIdentity)
        {
            using var step = this.prolongationStepsFactory();
            await step.Value.InvokeAsync(employeeId, eventId, endDate, userIdentity);

            await this.notification.SendNotification(
                eventId,
                employeeId,
                SickLeaveNotificationTemplate.SickLeaveProlonged,
                this.sickLeaveProlongNotificationConfiguration,
                CancellationToken.None);
        }

        public async Task CancelSickLeaveAsync(EmployeeId employeeId, int eventId, UserIdentity cancelledBy)
        {
            using var step = this.cancellationStepsFactory();
            await step.Value.InvokeAsync(employeeId, eventId, cancelledBy);

            await this.notification.SendNotification(
                eventId,
                employeeId,
                SickLeaveNotificationTemplate.SickLeaveCancelled,
                this.sickLeaveCancelNotificationConfiguration,
                CancellationToken.None);
        }

        private IQueryable<SickLeave> GetSickLeaves(ArcadiaCspContext context)
        {
            return context.SickLeaves
                .Include(x => x.SickLeaveCancellations)
                .Include(x => x.SickLeaveCompletes);
        }
    }
}