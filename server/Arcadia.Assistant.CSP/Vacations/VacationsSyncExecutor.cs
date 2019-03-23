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
        private const string VacationCloseReasonDataKey = "CloseReason";

        private readonly Func<ArcadiaCspContext> contextFactory;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public VacationsSyncExecutor(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IReadOnlyCollection<CalendarEventWithAdditionalData>> GetVacations()
        {
            using (var context = this.contextFactory())
            {
                var vacations = await this.GetVacationsInternal(context, trackChanges: false);

                var calendarEventsWithApprovals = vacations
                    .Where(v => v.Type == (int)VacationType.Regular)
                    .Select(this.CreateCalendarEventFromVacation)
                    .ToList();
                return calendarEventsWithApprovals;
            }
        }

        public async Task<CalendarEventWithAdditionalData> GetVacation(string employeeId, string vacationId)
        {
            using (var context = this.contextFactory())
            {
                var vacations = await this.GetVacationsInternal(
                    context,
                    employeeId,
                    vacationId,
                    false);

                var vacation = vacations.FirstOrDefault();
                return vacation != null
                    ? this.CreateCalendarEventFromVacation(vacation)
                    : null;
            }
        }

        public async Task<CalendarEventWithAdditionalData> InsertVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string createdBy)
        {
            this.logger.Debug($"Start adding new vacation from {@event.Dates.StartDate:d} to {@event.Dates.EndDate:d} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var vacation = this.CreateVacationFromCalendarEvent(@event, timestamp, createdBy);

                await context.Vacations.AddAsync(vacation);
                await context.SaveChangesAsync();

                this.logger.Debug(
                    $"New vacation with id {vacation.Id} is created in CSP database for vacation event from " +
                    $"{@event.Dates.StartDate:d} to {@event.Dates.EndDate:d} and employee {@event.EmployeeId}");

                return this.CreateCalendarEventFromVacation(vacation);
            }
        }

        public async Task<CalendarEventWithAdditionalData> UpdateVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy)
        {
            this.logger.Debug($"Start updating vacation with id {@event.EventId} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacationsInternal(
                    context,
                    @event.EmployeeId,
                    @event.EventId);

                var existingVacation = existingVacations.FirstOrDefault();
                var newVacation = this.CreateVacationFromCalendarEvent(@event, timestamp, updatedBy);

                if (existingVacation == null)
                {
                    throw new Exception($"Vacation with id {@event.EventId} is not found");
                }

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
                        existingApproval.IsFinal = approval.IsFinal;

                        if (existingApproval.TimeStamp == null)
                        {
                            existingApproval.TimeStamp = approval.TimeStamp;
                        }
                    }
                }

                context.Vacations.Update(existingVacation);
                await context.SaveChangesAsync();

                this.logger.Debug($"Vacation with id {existingVacation.Id} is updated");

                return this.CreateCalendarEventFromVacation(existingVacation);
            }
        }

        public async Task<Approval> UpsertVacationApproval(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string approvedBy)
        {
            this.logger.Debug($"Starting to upsert vacation approvals for event with id {@event.EventId} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacationsInternal(
                    context,
                    @event.EmployeeId,
                    @event.EventId);

                var existingVacation = existingVacations.FirstOrDefault();

                if (existingVacation == null)
                {
                    throw new Exception($"Vacation with id {@event.EventId} is not found");
                }

                var approvedById = int.Parse(approvedBy);

                this.EnsureVacationIsNotProcessed(existingVacation);

                VacationApprovals newApproval = null;

                var existingApproval = existingVacation.VacationApprovals
                    .FirstOrDefault(va => va.ApproverId == approvedById);
                if (existingApproval == null)
                {
                    this.logger.Debug($"New approval created for employee {approvedById} for vacation with id {existingVacation.Id}");

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

        private Task<List<Vacations>> GetVacationsInternal(
            ArcadiaCspContext context,
            string employeeId = null,
            string vacationId = null,
            bool trackChanges = true)
        {
            int? employeeDbId = null;
            if (int.TryParse(employeeId, out var tempEmployeeId))
            {
                employeeDbId = tempEmployeeId;
            }

            int? vacationDbId = null;
            if (int.TryParse(vacationId, out var tempVacationId))
            {
                vacationDbId = tempVacationId;
            }

            var vacations = context.Vacations
                .Include(v => v.VacationApprovals)
                .Include(v => v.VacationCancellations)
                .Include(v => v.VacationProcesses)
                .Where(v => (employeeId == null || v.EmployeeId == employeeDbId) && (vacationId == null || v.Id == vacationDbId));

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
                throw new Exception($"Vacation with id {vacation.Id} have been already processed and cannot be changed");
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
                var vacationCancellation = new VacationCancellations
                {
                    CancelledAt = timestamp,
                    CancelledById = updatedById
                };
                vacation.VacationCancellations.Add(vacationCancellation);
            }
            else if (@event.Status == VacationStatuses.Rejected)
            {
                var rejectedApproval = new VacationApprovals
                {
                    ApproverId = updatedById,
                    TimeStamp = timestamp,
                    Status = (int)VacationApprovalStatus.Declined
                };
                vacation.VacationApprovals.Add(rejectedApproval);
            }
            else if (@event.Status == VacationStatuses.Approved)
            {
                var finalApproval = new VacationApprovals
                {
                    ApproverId = updatedById,
                    TimeStamp = timestamp,
                    Status = (int)VacationApprovalStatus.Approved,
                    IsFinal = true
                };
                vacation.VacationApprovals.Add(finalApproval);
            }

            return vacation;
        }

        private CalendarEventWithAdditionalData CreateCalendarEventFromVacation(Vacations vacation)
        {
            var processed = vacation.VacationProcesses
                .Select(vp => new CalendarEventWithAdditionalData.VacationProcessing(vp.ProcessById.ToString(), vp.ProcessedAt))
                .FirstOrDefault();

            var cancelled = vacation.VacationCancellations
                .Select(vc => new CalendarEventWithAdditionalData.VacationCancellation(vc.CancelledById.ToString(), vc.CancelledAt, vc.Reason))
                .FirstOrDefault();

            var rejected = vacation.VacationApprovals
                .Where(va => va.Status == (int)VacationApprovalStatus.Declined)
                .Select(va => new CalendarEventWithAdditionalData.VacationRejection(va.ApproverId.ToString(), va.TimeStamp ?? DateTimeOffset.Now))
                .FirstOrDefault();

            var isApproved = rejected == null && vacation.VacationApprovals.Any(va => va.Status == (int)VacationApprovalStatus.Approved && va.IsFinal);

            string status = VacationStatuses.Requested;

            if (rejected != null)
            {
                status = VacationStatuses.Rejected;
            }
            else if (cancelled != null)
            {
                status = VacationStatuses.Cancelled;
            }
            else if (processed != null)
            {
                status = VacationStatuses.Processed;
            }
            else if (isApproved)
            {
                status = VacationStatuses.Approved;
            }

            Dictionary<string, string> additionalData = null;
            if (cancelled != null && !string.IsNullOrWhiteSpace(cancelled.CancelReason))
            {
                additionalData = new Dictionary<string, string>
                {
                    [VacationCloseReasonDataKey] = cancelled.CancelReason
                };
            }

            var calendarEvent = new CalendarEvent(
                vacation.Id.ToString(),
                CalendarEventTypes.Vacation,
                new DatesPeriod(vacation.Start.Date, vacation.End.Date),
                status,
                vacation.EmployeeId.ToString(),
                additionalData);

            var approvals = vacation.VacationApprovals
                .Where(va => va.Status == (int)VacationApprovalStatus.Approved)
                .Select(va => new Approval(va.TimeStamp ?? DateTimeOffset.Now, va.ApproverId.ToString()))
                .ToList();

            return new CalendarEventWithAdditionalData(calendarEvent, approvals, processed, cancelled, rejected);
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