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

        public async Task UpsertVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy,
            VacationsMatchInterval matchInterval)
        {
            this.logger.Debug($"Starting to synchronize vacation {@event.EventId} with CSP database");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacations(
                    context,
                    int.Parse(@event.EmployeeId),
                    matchInterval.Start,
                    matchInterval.End);

                this.logger.Debug(
                    $"{existingVacations.Count} vacations found in CSP database for match parameters: " +
                    $"employeeId {@event.EmployeeId}, startDate {matchInterval.Start}, endDate {matchInterval.End}");

                var vacation = this.CreateVacationFromCalendarEvent(@event, timestamp, updatedBy);

                if (existingVacations.Count == 0)
                {
                    this.logger.Debug("Created new vacation");
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

                        this.logger.Debug($"Updated vacation with id {existingVacation.Id}");
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
            this.logger.Debug($"Starting to synchronize approvals for vacation {@event.EventId} with CSP database");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacations(
                    context,
                    int.Parse(@event.EmployeeId),
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
                        this.logger.Debug($"New approval created for employee {approvedById}");

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
                this.logger.Debug($"Adding vacation cancellation by employee {updatedById}");
                vacation.CancelledAt = timestamp;
                vacation.CancelledById = updatedById;
            }

            if (@event.Status == VacationStatuses.Rejected)
            {
                var existingApproval = vacation.VacationApprovals
                    .FirstOrDefault(va => va.ApproverId == updatedById);

                if (existingApproval != null)
                {
                    this.logger.Debug($"Changing previously confirmed approval to declined by employee {updatedById}");
                    existingApproval.TimeStamp = timestamp;
                    existingApproval.Status = (int)VacationApprovalStatus.Declined;
                }
                else
                {
                    this.logger.Debug($"Adding declined approval by employee {updatedById}");
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