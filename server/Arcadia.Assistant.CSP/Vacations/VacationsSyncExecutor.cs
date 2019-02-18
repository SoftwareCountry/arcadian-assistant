namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    using NLog;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.CSP.Model;

    public class VacationsSyncExecutor
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public VacationsSyncExecutor(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IReadOnlyCollection<CalendarEventWithApprovals>> GetVacations(string employeeId)
        {
            using (var context = this.contextFactory())
            {
                var vacations = await this.GetVacationsInternal(context, int.Parse(employeeId));

                var calendarEventsWithApprovals = vacations
                    .Select(v =>
                    {
                        var calendarEvent = this.CreateCalendarEventFromVacation(v);
                        var approvals = v.VacationApprovals
                            .Where(va => va.Status == (int)VacationApprovalStatus.Approved)
                            .Select(va => new Approval(va.TimeStamp ?? DateTimeOffset.Now, va.ApproverId.ToString()))
                            .ToList();

                        return new CalendarEventWithApprovals(calendarEvent, approvals);
                    })
                    .ToList();
                return calendarEventsWithApprovals;
            }
        }

        public async Task UpsertVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy,
            VacationsMatchInterval matchInterval)
        {
            this.logger.Debug($"Starting to synchronize vacation {@event.EventId} for employee " +
                $"{@event.EmployeeId} with CSP database");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacationsInternal(
                    context,
                    int.Parse(@event.EmployeeId),
                    true,
                    matchInterval.Start,
                    matchInterval.End);

                this.logger.Debug(
                    $"{existingVacations.Count} vacations found in CSP database for match parameters: " +
                    $"employeeId {@event.EmployeeId}, startDate {matchInterval.Start}, endDate {matchInterval.End}");

                var vacation = this.CreateVacationFromCalendarEvent(@event, timestamp, updatedBy);

                if (existingVacations.Count == 0)
                {
                    this.logger.Debug($"Created new vacation in CSP database for event {@event.EventId} " +
                        $"and employee {@event.EmployeeId}");
                    await context.Vacations.AddAsync(vacation);
                }
                else
                {
                    foreach (var existingVacation in existingVacations)
                    {
                        existingVacation.Start = vacation.Start.Date;
                        existingVacation.End = vacation.End.Date;
                        //existingVacation.CancelledAt = vacation.CancelledAt;
                        //existingVacation.CancelledById = vacation.CancelledById;

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

                        this.logger.Debug($"Synchronized event {@event.EventId} to vacation with id {existingVacation.Id}");
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
            this.logger.Debug($"Starting to synchronize vacation approvals for event {@event.EventId} " +
                "with CSP database");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacationsInternal(
                    context,
                    int.Parse(@event.EmployeeId),
                    true,
                    matchInterval.Start,
                    matchInterval.End);

                this.logger.Debug(
                    $"{existingVacations.Count} vacations found in CSP database for match parameters: " +
                    $"employeeId {@event.EmployeeId}, startDate {matchInterval.Start}, endDate {matchInterval.End}");

                var approvedById = int.Parse(approvedBy);

                foreach (var existingVacation in existingVacations)
                {
                    var existingApproval = existingVacation.VacationApprovals
                        .FirstOrDefault(va => va.ApproverId == approvedById);
                    if (existingApproval == null)
                    {
                        this.logger.Debug($"New approval created for employee {approvedById} for vacation " +
                            $"with id {existingVacation.Id}");

                        existingVacation.VacationApprovals.Add(new VacationApprovals
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

        private Task<List<Vacations>> GetVacationsInternal(
            ArcadiaCspContext context,
            int employeeId,
            bool onlyActual = true,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            return context.Vacations
                .Include(v => v.VacationApprovals)
                .Include(v => v.VacationCancellations)
                .Include(v => v.VacationProcesses)
                .Where(v => v.EmployeeId == employeeId)
                .Where(v => startDate != null ? v.Start.Date == startDate : true)
                .Where(v => endDate != null ? v.End.Date == endDate : true)
                .Where(v => !onlyActual
                    ? true
                    : !v.VacationCancellations.Any() && v.VacationApprovals.All(va => va.Status != (int)VacationApprovalStatus.Declined))
                .ToListAsync();
        }

        private Vacations CreateVacationFromCalendarEvent(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy)
        {
            var vacation = new Vacations
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
                this.logger.Debug($"Adding vacation cancellation for event {@event.EventId} " +
                    $"by employee {updatedById}");
                //vacation.CancelledAt = timestamp;
                //vacation.CancelledById = updatedById;
            }

            if (@event.Status == VacationStatuses.Rejected)
            {
                var existingApproval = vacation.VacationApprovals
                    .FirstOrDefault(va => va.ApproverId == updatedById);

                if (existingApproval != null)
                {
                    this.logger.Debug("Changing previously confirmed approval to declined " +
                        $"for event {@event.EventId} by employee {updatedById}");
                    existingApproval.TimeStamp = timestamp;
                    existingApproval.Status = (int)VacationApprovalStatus.Declined;
                }
                else
                {
                    this.logger.Debug($"Adding declined approval for event {@event.EventId} " +
                        $"by employee {updatedById}");
                    var rejectedApproval = new VacationApprovals
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

        public CalendarEvent CreateCalendarEventFromVacation(Vacations vacation)
        {
            var isProcessed = vacation.VacationProcesses.Any();
            var isCancelled = vacation.VacationCancellations.Any();
            var isDeclined = vacation.VacationApprovals.Any(va => va.Status == (int)VacationApprovalStatus.Declined);

            string status = VacationStatuses.Requested;

            if (isCancelled)
            {
                status = VacationStatuses.Cancelled;
            }

            if (isDeclined)
            {
                status = VacationStatuses.Rejected;
            }

            if (isProcessed)
            {
                status = VacationStatuses.Processed;
            }

            var calendarEvent = new CalendarEvent(
                vacation.Id.ToString(),
                CalendarEventTypes.Vacation,
                new DatesPeriod(vacation.Start.Date, vacation.End.Date),
                status,
                vacation.EmployeeId.ToString());
            return calendarEvent;
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

        private enum VacationType
        {
            Regular = 0
        }

        private enum VacationApprovalStatus
        {
            Declined = 1,
            Approved = 2
        }
    }
}