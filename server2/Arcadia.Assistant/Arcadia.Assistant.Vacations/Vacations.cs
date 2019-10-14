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

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Vacations : StatelessService, IVacations
    {
        private readonly Func<Owned<ArcadiaCspContext>> cspFactory;
        private readonly Func<Owned<VacationsStorage>> storageFactory;

        private Dictionary<EmployeeId, Dictionary<int, VacationDescription>> vacations
            = new Dictionary<EmployeeId, Dictionary<int, VacationDescription>>();

        //private VacationsStorage storage = new VacationsStorage();

        public Vacations(StatelessServiceContext context, Func<Owned<ArcadiaCspContext>> cspFactory, Func<Owned<VacationsStorage>> storageFactory)
            : base(context)
        {
            this.cspFactory = cspFactory;
            this.storageFactory = storageFactory;
        }

        public async Task<VacationDescription[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using (var storage = this.storageFactory())
            {
                return await storage.Value.GetCalendarEvents(employeeId, cancellationToken);
            }
        }

        public async Task<VacationDescription> GetCalendarEventAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken)
        {
            using (var storage = this.storageFactory())
            {
                return await storage.Value.GetCalendarEvent(employeeId, eventId, cancellationToken);
            }
        }

        public async Task<VacationDescription> RequestVacationAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate)
        {
            using (var storage = this.storageFactory())
            {
                return await storage.Value.CreateCalendarEvent(employeeId, startDate, endDate);
            }
        }

        public async Task ChangeDatesAsync(EmployeeId employeeId, int eventId, DateTime startDate, DateTime endDate)
        {
            void Update(CSP.Model.Vacations oldValue)
            {
                var status = new StatusConverter().GetStatus(oldValue);
                if (status != VacationStatus.Requested)
                {
                    throw new InvalidOperationException($"A vacation {eventId} is not in Requested status ({status})");
                }

                oldValue.Start = startDate;
                oldValue.End = endDate;
            }

            using (var storage = this.storageFactory())
            {
                await storage.Value.UpdateCalendarEvent(employeeId, eventId, Update);
            }
        }

        public async Task CancelVacationAsync(EmployeeId employeeId, int eventId, EmployeeId cancelledBy, string cancellationReason)
        {
            void Update(CSP.Model.Vacations oldValue)
            {
                //var status = new StatusConverter().GetStatus(oldValue);
                oldValue.VacationCancellations.Add(
                    new VacationCancellations()
                    {
                        CancelledAt = DateTimeOffset.Now,
                        CancelledById = cancelledBy.Value,
                        Reason = cancellationReason
                    });
            }

            using (var storage = this.storageFactory())
            {
                await storage.Value.UpdateCalendarEvent(employeeId, eventId, Update);
            }
        }

        public async Task ApproveVacationAsync(EmployeeId employeeId, int eventId, EmployeeId approvedBy)
        {
            void Update(CSP.Model.Vacations oldValue)
            {
                //var status = new StatusConverter().GetStatus(oldValue);
                oldValue.VacationApprovals.Add(
                    new VacationApprovals()
                    {
                        ApproverId = approvedBy.Value,
                        IsFinal = true, //fix it
                        TimeStamp = DateTimeOffset.Now,
                        Status = 2
                    });
            }

            using (var storage = this.storageFactory())
            {
                await storage.Value.UpdateCalendarEvent(employeeId, eventId, Update);
            }
        }

        public async Task RejectVacationAsync(EmployeeId employeeId, int eventId, EmployeeId rejectedBy)
        {
            void Update(CSP.Model.Vacations oldValue)
            {
                //var status = new StatusConverter().GetStatus(oldValue);
                oldValue.VacationApprovals.Add(
                    new VacationApprovals()
                    {
                        ApproverId = rejectedBy.Value,
                        IsFinal = true, //fix it
                        TimeStamp = DateTimeOffset.Now,
                        Status = 1
                    });
            }

            using (var storage = this.storageFactory())
            {
                await storage.Value.UpdateCalendarEvent(employeeId, eventId, Update);
            }
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

                using (var csp = this.cspFactory())
                {
                    var actualVacations = await csp.Value.Vacations
                        .Include(x => x.VacationCancellations)
                        .Select(x => new EmployeeVacation
                        {
                            EmployeeId = new EmployeeId(x.EmployeeId),
                            Vacation = new VacationDescription
                            {
                                VacationId = x.Id,
                                StartDate = x.Start,
                                EndDate = x.End,
                                CancellationReason = x.VacationCancellations.Select(y => y.Reason).FirstOrDefault(),
                                Status =
                                    x.VacationCancellations.Any() ? VacationStatus.Cancelled
                                    : x.VacationApprovals.Any(v => v.Status == 1) ? VacationStatus.Rejected
                                    : x.VacationProcesses.Any() ? VacationStatus.Processed
                                    : x.VacationReadies.Any() ? VacationStatus.AccountingReady
                                    : x.VacationApprovals.Any(v => v.IsFinal && v.Status == 2) ? VacationStatus.Approved
                                    : VacationStatus.Requested
                            }
                        })
                        .ToListAsync(cancellationToken);
                }

                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
        }

        private void UpdateList(Dictionary<EmployeeId, Dictionary<int, VacationDescription>> databaseState)
        {
            var removedEmployees = this.vacations.Keys.Except(databaseState.Keys);
            foreach (var removedEmployee in removedEmployees)
            {
                this.vacations.Remove(removedEmployee);
            }

            foreach (var databaseEmployeeState in databaseState)
            {
                var localEmployeeState = this.vacations[databaseEmployeeState.Key];
                foreach (var removedVacation in localEmployeeState.Keys.Except(databaseEmployeeState.Value.Keys))
                {
                    localEmployeeState.Remove(removedVacation);
                }

                foreach (var databaseRecord in databaseEmployeeState.Value)
                {
                    var eventId = databaseRecord.Key;
                    var databaseVacation = databaseRecord.Value;
                    if (localEmployeeState.TryGetValue(eventId, out var localVacation))
                    {
                        if (localVacation.StartDate != databaseVacation.StartDate || localVacation.EndDate != databaseVacation.EndDate)
                        {
                            //TODO: dates changed
                        }

                        if (localVacation.Status != databaseVacation.Status)
                        {
                            //TODO: status changed
                        }
                    }
                    else
                    {
                        // TODO: vacation added, notify
                    }

                    localEmployeeState[eventId] = databaseRecord.Value;
                }
            }
        }

        private class EmployeeVacation
        {
            public EmployeeId EmployeeId { get; set; }

            public VacationDescription Vacation { get; set; }
        }
    }
}