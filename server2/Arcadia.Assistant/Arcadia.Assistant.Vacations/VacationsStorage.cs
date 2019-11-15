namespace Arcadia.Assistant.Vacations
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP.Model;

    using Employees.Contracts;

    using Microsoft.EntityFrameworkCore;

    using VacationApproval = Contracts.VacationApproval;

    public class VacationsStorage
    {
        private readonly Func<Owned<ArcadiaCspContext>> cspFactory;

        public VacationsStorage(Func<Owned<ArcadiaCspContext>> cspFactory)
        {
            this.cspFactory = cspFactory;
        }

        public async Task<VacationDescription[]> GetCalendarEvents(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var csp = this.cspFactory();
            return await csp.Value.Vacations
                .Where(x => x.EmployeeId == employeeId.Value)
                .Select(this.toDescription)
                .ToArrayAsync(cancellationToken);
        }

        public async Task<VacationDescription> GetCalendarEvent(EmployeeId employeeId, int eventId, CancellationToken cancellationToken)
        {
            using var csp = this.cspFactory();
            return await csp.Value.Vacations
                .Where(x => x.EmployeeId == employeeId.Value && eventId == x.Id)
                .Select(this.toDescription)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<VacationDescription> CreateCalendarEvent(EmployeeId employeeId, DateTime startDate, DateTime endDate)
        {
            using var csp = this.cspFactory();
            var model = new CSP.Model.Vacation
            {
                EmployeeId = employeeId.Value,
                RaisedAt = DateTimeOffset.Now,
                Start = startDate,
                End = endDate
            };

            csp.Value.Vacations.Add(model);

            await csp.Value.SaveChangesAsync();

            var newVacation = new VacationDescription
            {
                EmployeeId = employeeId,
                StartDate = model.Start,
                EndDate = model.End,
                VacationId = model.Id
            };

            return newVacation;
        }


        public async Task UpdateCalendarEvent(EmployeeId employeeId, int eventId, Action<CSP.Model.Vacation> updateFunc)
        {
            using var csp = this.cspFactory();
            var vacationInstance = await this.LoadVacationInstance(csp.Value, employeeId, eventId);
            updateFunc(vacationInstance);
            await csp.Value.SaveChangesAsync();
        }

        private async Task<CSP.Model.Vacation> LoadVacationInstance(ArcadiaCspContext context, EmployeeId employeeId, int eventId)
        {
            var model = await context
                .Vacations
                .Include(x => x.VacationCancellations)
                .Include(x => x.VacationApprovals)
                .Include(x => x.VacationProcesses)
                .Include(x => x.VacationReadies)
                .AsTracking()
                .FirstOrDefaultAsync(x => x.EmployeeId == employeeId.Value && x.Id == eventId);

            if (model == null)
            {
                throw new ArgumentException($"Event {eventId} not found for {employeeId}", nameof(eventId));
            }

            return model;
        }

        private readonly Expression<Func<CSP.Model.Vacation, VacationDescription>> toDescription = x => new VacationDescription()
        {
            EmployeeId = new EmployeeId(x.EmployeeId),
            CancellationReason = x.VacationCancellations.Select(y => y.Reason).FirstOrDefault(),
            StartDate = x.Start,
            EndDate = x.End,
            VacationId = x.Id,
            Approvals = x.VacationApprovals
                .Where(v => v.Status == VacationApprovalStatus.Approved)
                .Select(v => new VacationApproval(new EmployeeId(v.Id)) { IsFinal = v.IsFinal, Timestamp = v.TimeStamp })
                .ToArray(),

            IsRejected = x.VacationApprovals.Any(v => v.Status == VacationApprovalStatus.Rejected),
            IsProcessed = x.VacationProcesses.Any(),
            IsCancelled = x.VacationCancellations.Any(),
            AccountingReady = x.VacationReadies.Any(),

            /*
            Status = x.VacationCancellations.Any() ? VacationStatus.Cancelled
                    : x.VacationApprovals.Any(v => v.Status == 1) ? VacationStatus.Rejected
                    : x.VacationProcesses.Any() ? VacationStatus.Processed
                    : x.VacationReadies.Any() ? VacationStatus.AccountingReady
                    : x.VacationApprovals.Any(v => v.IsFinal && v.Status == 2) ? VacationStatus.Approved //TODO: status should be calculated separately
                    : VacationStatus.Requested*/
        };

        public async Task<Dictionary<EmployeeId, VacationDescription[]>> GetCalendarEvents(EmployeeId[] employeeIds, CancellationToken cancellationToken)
        {
            using var csp = this.cspFactory();
            var ids = employeeIds.Select(x => x.Value);
            var pendingVacations = await csp.Value.Vacations
                .Where(x => ids.Contains(x.EmployeeId))
                .Select(this.toDescription)
                .ToArrayAsync(cancellationToken);

            return pendingVacations.GroupBy(x => x.EmployeeId).ToDictionary(x => x.Key, x => x.ToArray());
        }
    }
}