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
                var vacations = await this.GetMatchingVacations(context, int.Parse(employeeId), trackChanges: false);

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

        public async Task<CalendarEvent> UpsertVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy,
            VacationsMatchInterval matchInterval)
        {
            this.logger.Debug(
                $"Starting to upsert vacation event from {@event.Dates.StartDate} to {@event.Dates.EndDate} " +
                $"for employee {@event.EmployeeId} matching dates from {matchInterval.Start} to {matchInterval.End}");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetMatchingVacations(
                    context,
                    int.Parse(@event.EmployeeId),
                    startDate: matchInterval.Start,
                    endDate: matchInterval.End);

                if (existingVacations.Count > 1)
                {
                    throw new Exception(
                        "There are multiple actual vacations in CSP database matching dates from " +
                        $"{matchInterval.Start} to {matchInterval.End}");
                }

                var existingVacation = existingVacations.FirstOrDefault();
                var newVacation = this.CreateVacationFromCalendarEvent(@event, timestamp, updatedBy);

                if (existingVacation != null)
                {
                    this.logger.Debug(
                        $"Vacation with id {existingVacation.Id} is found in CSP database for match parameters: " +
                        $"employeeId {@event.EmployeeId}, startDate {matchInterval.Start}, endDate {matchInterval.End}");

                    this.EnsureVacationIsNotProcessed(existingVacation);

                    existingVacation.Start = newVacation.Start.Date;
                    existingVacation.End = newVacation.End.Date;

                    foreach (var cancellation in newVacation.VacationCancellations)
                    {
                        var existingCancellation = existingVacation.VacationCancellations
                            .FirstOrDefault(vc => vc.CancelledById == cancellation.CancelledById);
                        if (existingCancellation == null)
                        {
                            existingVacation.VacationCancellations.Add(cancellation);
                        }
                    }

                    foreach (var approval in newVacation.VacationApprovals)
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

                    this.logger.Debug($"Vacation from {@event.Dates.StartDate} to {@event.Dates.EndDate} " +
                        $"for employee {@event.EmployeeId} is updated in CSP database vacation with id {existingVacation.Id}");
                }
                else
                {
                    existingVacation = newVacation;
                    await context.Vacations.AddAsync(newVacation);

                    this.logger.Debug($"New vacation is created in CSP database for event {@event.EventId} " +
                        $"and employee {@event.EmployeeId}");
                }

                await context.SaveChangesAsync();

                return this.CreateCalendarEventFromVacation(existingVacation);
            }
        }

        public async Task<Approval> UpsertVacationApproval(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string approvedBy,
            VacationsMatchInterval matchInterval)
        {
            this.logger.Debug($"Starting to upsert vacation approvals for event with id {@event.EventId} to CSP database");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetMatchingVacations(
                    context,
                    int.Parse(@event.EmployeeId),
                    int.Parse(@event.EventId));

                var existingVacation = existingVacations.FirstOrDefault();

                if (existingVacation == null)
                {
                    throw new Exception($"There are no vacations in CSP database with id {@event.EventId}");
                }

                var approvedById = int.Parse(approvedBy);

                this.EnsureVacationIsNotProcessed(existingVacation);

                VacationApprovals newApproval = null;

                var existingApproval = existingVacation.VacationApprovals
                    .FirstOrDefault(va => va.ApproverId == approvedById);
                if (existingApproval == null)
                {
                    this.logger.Debug($"New approval created for employee {approvedById} for vacation " +
                        $"with id {existingVacation.Id}");

                    newApproval = new VacationApprovals
                    {
                        TimeStamp = timestamp,
                        ApproverId = approvedById,
                        Status = (int)VacationApprovalStatus.Approved
                    };
                    existingVacation.VacationApprovals.Add(newApproval);
                }

                context.Vacations.Update(existingVacation);

                await context.SaveChangesAsync();

                return newApproval != null
                    ? new Approval(newApproval.TimeStamp ?? DateTimeOffset.Now, newApproval.ApproverId.ToString())
                    : null;
            }
        }

        private Task<List<Vacations>> GetMatchingVacations(
            ArcadiaCspContext context,
            int employeeId,
            int? vacationId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            bool trackChanges = true)
        {
            var vacations = context.Vacations
                .Include(v => v.VacationApprovals)
                .Include(v => v.VacationCancellations)
                .Include(v => v.VacationProcesses)
                .Where(v =>
                    v.EmployeeId == employeeId &&
                    (vacationId == null || v.Id == vacationId) &&
                    (startDate == null || v.Start.Date == startDate) &&
                    (endDate == null || v.End.Date == endDate))
                .Where(v => !v.VacationCancellations.Any() && v.VacationApprovals.All(va => va.Status != (int)VacationApprovalStatus.Declined));

            if (!trackChanges)
            {
                vacations = vacations.AsNoTracking();
            }

            return vacations.ToListAsync();
        }

        private void EnsureVacationIsNotProcessed(Vacations vacation)
        {
            if (vacation.VacationProcesses.Any())
            {
                throw new Exception($"Vacation from {vacation.Start.Date} to {vacation.End.Date} is already processed and cannot be changed");
            }
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

                var vacationCancellation = new VacationCancellations
                {
                    CancelledAt = timestamp,
                    CancelledById = updatedById
                };
                vacation.VacationCancellations.Add(vacationCancellation);
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

        private CalendarEvent CreateCalendarEventFromVacation(Vacations vacation)
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