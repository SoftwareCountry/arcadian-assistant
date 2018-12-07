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

        public async Task<Vacation> SyncVacation(CalendarEvent @event, IEnumerable<string> eventApprovals, VacationsMatchInterval matchInterval)
        {
            var vacation = this.CreateVacationFromCalendarEvent(@event, eventApprovals);

            using (var context = this.contextFactory())
            {
                var existingVacation = await this.GetVacation(
                    context,
                    int.Parse(@event.EmployeeId),
                    matchInterval.Start,
                    matchInterval.End);

                if (existingVacation == null)
                {
                    existingVacation = (await context.Vacations.AddAsync(vacation)).Entity;
                }
                else
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

                    context.Vacations.Update(existingVacation);
                }

                await context.SaveChangesAsync();
                return existingVacation;
            }
        }

        private Task<Vacation> GetVacation(
            ArcadiaCspContext context,
            int employeeId,
            DateTime startDate,
            DateTime endDate)
        {
            return context.Vacations
                .Include(v => v.VacationApprovals)
                .FirstOrDefaultAsync(v =>
                    v.EmployeeId == employeeId &&
                    v.Start.Date == startDate.Date &&
                    v.End.Date == endDate.Date);
        }

        private Vacation CreateVacationFromCalendarEvent(CalendarEvent @event, IEnumerable<string> approvals)
        {
            var vacation = new Vacation
            {
                EmployeeId = int.Parse(@event.EmployeeId),
                RaisedAt = DateTimeOffset.Now,
                Start = @event.Dates.StartDate.Date,
                End = @event.Dates.EndDate.Date,
                Type = this.GetRegularVacationType()
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
                    Status = this.GetApprovalApprovedStatus()
                });

            foreach (var vacationApproval in vacationApprovals)
            {
                vacation.VacationApprovals.Add(vacationApproval);
            }

            return vacation;
        }

        // ToDo: get it from database
        private int GetRegularVacationType()
        {
            return 0;
        }

        // ToDo: get it from database
        private int GetApprovalApprovedStatus()
        {
            return 2;
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
    }
}