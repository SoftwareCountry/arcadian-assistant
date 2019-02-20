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
                var vacations = await this.GetVacationsInternal(context, int.Parse(employeeId), trackChanges: false);

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

        public async Task<CalendarEventWithApprovals> InsertVacation(
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

                var result = new CalendarEventWithApprovals(
                    this.CreateCalendarEventFromVacation(vacation),
                    Enumerable.Empty<Approval>());
                return result;
            }
        }

        public async Task<CalendarEventWithApprovals> UpdateVacation(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy)
        {
            this.logger.Debug($"Start updating vacation with id {@event.EventId} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var existingVacations = await this.GetVacationsInternal(
                    context,
                    int.Parse(@event.EmployeeId),
                    int.Parse(@event.EventId));

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

                var resultEvent = this.CreateCalendarEventFromVacation(existingVacation);
                var resultApprovals = existingVacation.VacationApprovals
                    .Where(va => va.Status == (int)VacationApprovalStatus.Approved)
                    .Select(va => new Approval(va.TimeStamp ?? DateTimeOffset.Now, va.ApproverId.ToString()))
                    .ToList();

                return new CalendarEventWithApprovals(resultEvent, resultApprovals);
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
                    int.Parse(@event.EmployeeId),
                    int.Parse(@event.EventId));

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
            int employeeId,
            int? vacationId = null,
            bool trackChanges = true)
        {
            var vacations = context.Vacations
                .Include(v => v.VacationApprovals)
                .Include(v => v.VacationCancellations)
                .Include(v => v.VacationProcesses)
                .Where(v => v.EmployeeId == employeeId && (vacationId == null || v.Id == vacationId))
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

        private CalendarEvent CreateCalendarEventFromVacation(Vacations vacation)
        {
            var isProcessed = vacation.VacationProcesses.Any();
            var isCancelled = vacation.VacationCancellations.Any();
            var isDeclined = vacation.VacationApprovals.Any(va => va.Status == (int)VacationApprovalStatus.Declined);
            var isApproved = !isDeclined && vacation.VacationApprovals.Any(va => va.Status == (int)VacationApprovalStatus.Approved && va.IsFinal);

            string status = VacationStatuses.Requested;

            if (isProcessed)
            {
                status = VacationStatuses.Processed;
            }
            else if (isDeclined)
            {
                status = VacationStatuses.Rejected;
            }
            else if (isCancelled)
            {
                status = VacationStatuses.Cancelled;
            }
            else if (isApproved)
            {
                status = VacationStatuses.Approved;
            }

            var calendarEvent = new CalendarEvent(
                vacation.Id.ToString(),
                CalendarEventTypes.Vacation,
                new DatesPeriod(vacation.Start.Date, vacation.End.Date),
                status,
                vacation.EmployeeId.ToString());
            return calendarEvent;
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