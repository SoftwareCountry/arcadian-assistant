﻿namespace Arcadia.Assistant.Vacations
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
                Status = VacationStatus.Requested,
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
            CancellationReason = x.VacationCancellations.Select(y => y.Reason).FirstOrDefault(),
            StartDate = x.Start,
            EndDate = x.End,
            VacationId = x.Id,
            Status = x.VacationCancellations.Any() ? VacationStatus.Cancelled
                    : x.VacationApprovals.Any(v => v.Status == 1) ? VacationStatus.Rejected
                    : x.VacationProcesses.Any() ? VacationStatus.Processed
                    : x.VacationReadies.Any() ? VacationStatus.AccountingReady
                    : x.VacationApprovals.Any(v => v.IsFinal && v.Status == 2) ? VacationStatus.Approved
                    : VacationStatus.Requested
        };
    }
}