namespace Arcadia.Assistant.Vacations
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

    using VacationApproval = Contracts.VacationApproval;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Vacations : StatelessService, IVacations
    {
        private readonly Func<Owned<VacationsStorage>> storageFactory;
        private readonly VacationChangesWatcher changesWatcher;

        //private VacationsStorage storage = new VacationsStorage();

        public Vacations(
            StatelessServiceContext context,
            Func<Owned<VacationsStorage>> storageFactory,
            VacationChangesWatcher changesWatcher)
            : base(context)
        {
            this.storageFactory = storageFactory;
            this.changesWatcher = changesWatcher;
        }

        public async Task<VacationDescription[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var storage = this.storageFactory();
            return await storage.Value.GetCalendarEvents(employeeId, cancellationToken);
        }

        public async Task<VacationDescription?> GetCalendarEventAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken)
        {
            using var storage = this.storageFactory();
            return await storage.Value.GetCalendarEvent(employeeId, eventId, cancellationToken);
        }

        public async Task<VacationDescription> RequestVacationAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate)
        {
            using var storage = this.storageFactory();
            var description = await storage.Value.CreateCalendarEvent(employeeId, startDate, endDate);
            this.changesWatcher.ForceRefresh();
            return description;
        }

        public async Task ChangeDatesAsync(EmployeeId employeeId, int eventId, DateTime startDate, DateTime endDate)
        {
            void Update(Vacation oldValue)
            {
                var status = new StatusConverter().GetStatus(oldValue);
                if (status != VacationStatus.Requested)
                {
                    throw new InvalidOperationException($"A vacation {eventId} is not in Requested status ({status})");
                }

                oldValue.Start = startDate;
                oldValue.End = endDate;
            }

            await this.UpdateCalendarEvent(employeeId, eventId, Update);
        }

        public async Task CancelVacationAsync(EmployeeId employeeId, int eventId, EmployeeId cancelledBy, string cancellationReason)
        {
            void Update(Vacation oldValue)
            {
                //var status = new StatusConverter().GetStatus(oldValue);
                oldValue.VacationCancellations.Add(
                    new VacationCancellation()
                    {
                        CancelledAt = DateTimeOffset.Now,
                        CancelledById = cancelledBy.Value,
                        Reason = cancellationReason
                    });
            }

            await this.UpdateCalendarEvent(employeeId, eventId, Update);
        }

        public async Task ApproveVacationAsync(EmployeeId employeeId, int eventId, EmployeeId approvedBy)
        {
            void Update(Vacation oldValue)
            {
                //var status = new StatusConverter().GetStatus(oldValue);
                oldValue.VacationApprovals.Add(
                    new CSP.Model.VacationApproval()
                    {
                        ApproverId = approvedBy.Value,
                        IsFinal = true, //fix it
                        TimeStamp = DateTimeOffset.Now,
                        Status = 2
                    });
            }

            await this.UpdateCalendarEvent(employeeId, eventId, Update);
        }

        public async Task RejectVacationAsync(EmployeeId employeeId, int eventId, EmployeeId rejectedBy)
        {
            void Update(Vacation oldValue)
            {
                //var status = new StatusConverter().GetStatus(oldValue);
                oldValue.VacationApprovals.Add(
                    new CSP.Model.VacationApproval()
                    {
                        ApproverId = rejectedBy.Value,
                        IsFinal = true, //fix it
                        TimeStamp = DateTimeOffset.Now,
                        Status = 1
                    });
            }

            await this.UpdateCalendarEvent(employeeId, eventId, Update);
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
            await this.changesWatcher.StartAsync(cancellationToken);
        }

        private async Task UpdateCalendarEvent(EmployeeId employeeId, int eventId, Action<Vacation> updateFunc)
        {
            using var storage = this.storageFactory();
            await storage.Value.UpdateCalendarEvent(employeeId, eventId, updateFunc);
            this.changesWatcher.ForceRefresh();
        }
    }
}