namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.CSP.Model;

    public class VacationsSyncExecutor
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public VacationsSyncExecutor(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task UpsertVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy,
            VacationsMatchInterval matchInterval)
        {
            var vacation = this.CreateVacationFromCalendarEvent(@event, timestamp, updatedBy);

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacations(
                    context,
                    int.Parse(@event.EmployeeId),
                    matchInterval.Start,
                    matchInterval.End);

                if (existingVacations.Count == 0)
                {
                    await context.Vacations.AddAsync(vacation);
                }
                else
                {
                    foreach (var existingVacation in existingVacations)
                    {
                        existingVacation.Start = vacation.Start.Date;
                        existingVacation.End = vacation.End.Date;
                        existingVacation.CancelledAt = vacation.CancelledAt;
                        existingVacation.CancelledById = vacation.CancelledById;

                        foreach (var approval in vacation.VacationApprovals)
                        {
                            var existingApproval = existingVacation.VacationApprovals
                                .FirstOrDefault(va => va.ApproverId == approval.ApproverId);
                            if (existingApproval == null)
                            {
                                existingVacation.VacationApprovals.Add(approval);
                            }
                            else
                            {
                                existingApproval.Status = approval.Status;

                                if (existingApproval.TimeStamp == null)
                                {
                                    existingApproval.TimeStamp = approval.TimeStamp;
                                }
                            }
                        }
                    }

                    context.Vacations.UpdateRange(existingVacations);
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task UpsertVacationApproval(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string approvedBy,
            VacationsMatchInterval matchInterval)
        {
            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacations(
                    context,
                    int.Parse(@event.EmployeeId),
                    matchInterval.Start,
                    matchInterval.End);

                var approvedById = int.Parse(approvedBy);

                foreach (var existingVacation in existingVacations)
                {
                    var existingApproval = existingVacation.VacationApprovals
                        .FirstOrDefault(va => va.ApproverId == approvedById);
                    if (existingApproval == null)
                    {
                        existingVacation.VacationApprovals.Add(new VacationApproval
                        {
                            TimeStamp = timestamp,
                            ApproverId = approvedById,
                            Status = (int)VacationApprovalStatus.Approved
                        });
                    }
                }

                context.Vacations.UpdateRange(existingVacations);
                await context.SaveChangesAsync();
            }
        }

        private Task<List<Vacation>> GetVacations(
        ArcadiaCspContext context,
        int employeeId,
        DateTime startDate,
        DateTime endDate)
        {
            return context.Vacations
                .Include(v => v.VacationApprovals)
                .Where(v =>
                    v.EmployeeId == employeeId &&
                    v.Start.Date == startDate.Date &&
                    v.End.Date == endDate.Date &&
                    v.CancelledAt == null &&
                    v.VacationApprovals.All(va => va.Status != (int)VacationApprovalStatus.Declined))
                .ToListAsync();
        }

        private Vacation CreateVacationFromCalendarEvent(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy)
        {
            var vacation = new Vacation
            {
                EmployeeId = int.Parse(@event.EmployeeId),
                RaisedAt = timestamp,
                Start = @event.Dates.StartDate.Date,
                End = @event.Dates.EndDate.Date,
                Type = (int)VacationType.Regular
            };

            var updatedById = int.Parse(updatedBy);

            if (@event.Status == VacationStatuses.Cancelled)
            {
                vacation.CancelledAt = timestamp;
                vacation.CancelledById = updatedById;
            }

            if (@event.Status == VacationStatuses.Rejected)
            {
                var existingApproval = vacation.VacationApprovals
                    .FirstOrDefault(va => va.ApproverId == updatedById);

                if (existingApproval != null)
                {
                    existingApproval.TimeStamp = timestamp;
                    existingApproval.Status = (int)VacationApprovalStatus.Declined;
                }
                else
                {
                    var rejectedApproval = new VacationApproval
                    {
                        ApproverId = updatedById,
                        TimeStamp = timestamp,
                        Status = (int)VacationApprovalStatus.Declined
                    };
                    vacation.VacationApprovals.Add(rejectedApproval);
                }
            }

            return vacation;
        }

        public class VacationsMatchInterval
        {
            public VacationsMatchInterval(DateTime start, DateTime end)
            {
                this.Start = start;
                this.End = end;
            }

            public DateTime Start { get; }

            public DateTime End { get; }
        }

        // ToDo: get it from database
        private enum VacationType
        {
            Regular = 0
        }

        // ToDo: get it from database
        private enum VacationApprovalStatus
        {
            Declined = 1,
            Approved = 2
        }
    }
}