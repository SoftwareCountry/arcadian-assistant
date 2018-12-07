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

        public async Task<IEnumerable<Vacation>> SyncVacation(CalendarEvent @event, IEnumerable<string> eventApprovals, VacationsMatchInterval matchInterval)
        {
            var vacation = this.CreateVacationFromCalendarEvent(@event, eventApprovals);

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

        private Vacation CreateVacationFromCalendarEvent(CalendarEvent @event, IEnumerable<string> approvals)
        {
            var vacation = new Vacation
            {
                EmployeeId = int.Parse(@event.EmployeeId),
                RaisedAt = DateTimeOffset.Now,
                Start = @event.Dates.StartDate.Date,
                End = @event.Dates.EndDate.Date,
                Type = (int)VacationType.Regular
            };

            if (@event.Status == VacationStatuses.Cancelled)
            {
                vacation.CancelledAt = DateTimeOffset.Now;
            }

            var vacationApprovals = approvals
                .Select(approval => new VacationApproval
                {
                    ApproverId = int.Parse(approval),
                    TimeStamp = DateTimeOffset.Now,
                    Status = (int)VacationApprovalStatus.Approved
                })
                .ToList();

            if (@event.Status == VacationStatuses.Rejected)
            {
                var rejectedApproval = new VacationApproval
                {
                    ApproverId = int.Parse(@event.UpdateEmployeeId),
                    TimeStamp = DateTimeOffset.Now,
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