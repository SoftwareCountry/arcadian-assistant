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

        public async Task<IEnumerable<Vacation>> UpsertVacation(CalendarEvent @event, VacationApprovals approvals, VacationsMatchInterval matchInterval)
        {
            var vacation = this.CreateVacationFromCalendarEvent(@event, approvals);

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacations(
                    context,
                    int.Parse(@event.EmployeeId),
                    matchInterval.Start,
                    matchInterval.End);

                if (existingVacations.Count == 0)
                {
                    var createdVacation = (await context.Vacations.AddAsync(vacation)).Entity;
                    existingVacations = new List<Vacation> { createdVacation };
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
                return existingVacations;
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

        private Vacation CreateVacationFromCalendarEvent(CalendarEvent @event, VacationApprovals approvals)
        {
            var vacation = new Vacation
            {
                EmployeeId = int.Parse(@event.EmployeeId),
                RaisedAt = @event.CreateDate,
                Start = @event.Dates.StartDate.Date,
                End = @event.Dates.EndDate.Date,
                Type = (int)VacationType.Regular
            };

            if (@event.Status == VacationStatuses.Cancelled)
            {
                vacation.CancelledAt = @event.UpdateDate;
                vacation.CancelledById = int.Parse(@event.UpdateEmployeeId);
            }

            var vacationApprovals = approvals.Approvals
                .Select(approval => new VacationApproval
                {
                    ApproverId = int.Parse(approval),
                    TimeStamp = approval == approvals.LastApproverId ? approvals.LastApprovalDate : DateTimeOffset.Now,
                    Status = (int)VacationApprovalStatus.Approved
                })
                .ToList();

            if (@event.Status == VacationStatuses.Rejected)
            {
                var rejectedApproval = new VacationApproval
                {
                    ApproverId = int.Parse(@event.UpdateEmployeeId),
                    TimeStamp = @event.UpdateDate,
                    Status = (int)VacationApprovalStatus.Declined
                };
                vacationApprovals.Add(rejectedApproval);
            }

            foreach (var vacationApproval in vacationApprovals)
            {
                vacation.VacationApprovals.Add(vacationApproval);
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

        public class VacationApprovals
        {
            public VacationApprovals()
                : this(null, null, new string[0])
            {
            }

            public VacationApprovals(string lastApproverId, DateTimeOffset? lastApprovalDate, IEnumerable<string> approvals)
            {
                this.LastApproverId = lastApproverId;
                this.LastApprovalDate = lastApprovalDate;
                this.Approvals = approvals;
            }

            public string LastApproverId { get; set; }

            public DateTimeOffset? LastApprovalDate { get; set; }

            public IEnumerable<string> Approvals { get; set; }
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