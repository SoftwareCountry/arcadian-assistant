namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.CSP.Model;

    using Microsoft.EntityFrameworkCore;

    using NLog;

    public class SickLeavesSyncExecutor
    {
        private readonly Func<ArcadiaCspContext> contextFactory;
        private readonly CspCalendarEventIdParser calendarEventIdParser;

        private readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public SickLeavesSyncExecutor(Func<ArcadiaCspContext> contextFactory, CspCalendarEventIdParser calendarEventIdParser)
        {
            this.contextFactory = contextFactory;
            this.calendarEventIdParser = calendarEventIdParser;
        }

        public async Task<IReadOnlyCollection<CalendarEventWithAdditionalData>> GetSickLeaves()
        {
            using (var context = this.contextFactory())
            {
                var sickLeaves = await this.GetSickLeavesInternal(context, trackChanges: false);

                var calendarEventsWithApprovals = sickLeaves
                    .Select(this.CreateCalendarEventFromSickLeave)
                    .ToList();
                return calendarEventsWithApprovals;
            }
        }

        public async Task<CalendarEventWithAdditionalData> GetSickLeave(string employeeId, string sickLeaveId)
        {
            using (var context = this.contextFactory())
            {
                var sickLeaves = await this.GetSickLeavesInternal(
                    context,
                    employeeId,
                    sickLeaveId,
                    false);

                var sickLeave = sickLeaves.FirstOrDefault();

                return sickLeave != null
                    ? this.CreateCalendarEventFromSickLeave(sickLeave)
                    : null;
            }
        }

        public async Task<CalendarEventWithAdditionalData> InsertSickLeave(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string createdBy)
        {
            this.logger.Debug($"Start adding new sick leave from {@event.Dates.StartDate:d} to {@event.Dates.EndDate:d} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var sickLeave = this.CreateSickLeaveFromCalendarEvent(@event, timestamp, createdBy);

                await context.SickLeaves.AddAsync(sickLeave);
                await context.SaveChangesAsync();

                this.logger.Debug(
                    $"New sick leave with id {sickLeave.Id} is created in CSP database for sick leave event from " +
                    $"{@event.Dates.StartDate:d} to {@event.Dates.EndDate:d} and employee {@event.EmployeeId}");

                return this.CreateCalendarEventFromSickLeave(sickLeave);
            }
        }

        public async Task<CalendarEventWithAdditionalData> UpdateSickLeave(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy)
        {
            this.logger.Debug($"Start updating sick leave with id {@event.EventId} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var existingSickLeaves = await this.GetSickLeavesInternal(
                    context,
                    @event.EmployeeId,
                    @event.EventId);

                var existingSickLeave = existingSickLeaves.FirstOrDefault();
                if (existingSickLeave == null)
                {
                    throw new Exception($"Sick leave with id {@event.EventId} is not found");
                }

                this.EnsureSickLeaveIsNotCompleted(existingSickLeave);

                var newSickLeave = this.CreateSickLeaveFromCalendarEvent(@event, timestamp, updatedBy);
                existingSickLeave.Start = newSickLeave.Start.Date;
                existingSickLeave.End = newSickLeave.End.Date;

                var existingEvent = this.CreateCalendarEventFromSickLeave(existingSickLeave);

                if (@event.Status != existingEvent.CalendarEvent.Status)
                {
                    foreach (var cancellation in newSickLeave.SickLeaveCancellations)
                    {
                        var existingCancellation = existingSickLeave.SickLeaveCancellations
                            .FirstOrDefault(c => c.ById == cancellation.ById);

                        if (existingCancellation == null)
                        {
                            existingSickLeave.SickLeaveCancellations.Add(cancellation);
                        }
                    }

                    foreach (var approval in newSickLeave.SickLeaveAccepts)
                    {
                        var existingApproval = existingSickLeave.SickLeaveAccepts
                            .FirstOrDefault(a => a.ById == approval.ById);

                        if (existingApproval == null)
                        {
                            existingSickLeave.SickLeaveAccepts.Add(approval);
                        }
                    }

                    foreach (var rejection in newSickLeave.SickLeaveRejects)
                    {
                        var existingRejection = existingSickLeave.SickLeaveRejects
                            .FirstOrDefault(r => r.ById == rejection.ById);

                        if (existingRejection == null)
                        {
                            existingSickLeave.SickLeaveRejects.Add(rejection);
                        }
                    }
                }

                context.SickLeaves.Update(existingSickLeave);
                await context.SaveChangesAsync();

                this.logger.Debug($"Sick leave with id {existingSickLeave.Id} is updated");

                return this.CreateCalendarEventFromSickLeave(existingSickLeave);
            }
        }

        public async Task<Approval> UpsertSickLeaveApproval(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string approvedBy)
        {
            this.logger.Debug($"Starting to upsert sick leave approval for event with id {@event.EventId} for employee {@event.EmployeeId}");

            using (var context = this.contextFactory())
            {
                var existingSickLeaves = await this.GetSickLeavesInternal(
                    context,
                    @event.EmployeeId,
                    @event.EventId);

                var existingSickLeave = existingSickLeaves.FirstOrDefault();
                if (existingSickLeave == null)
                {
                    throw new Exception($"Sick leave with id {@event.EventId} is not found");
                }

                this.EnsureSickLeaveIsNotCompleted(existingSickLeave);

                var approvedById = int.Parse(approvedBy);

                SickLeaveAccepts newApproval = null;

                var existingApproval = existingSickLeave.SickLeaveAccepts
                    .FirstOrDefault(va => va.ById == approvedById);

                if (existingApproval == null)
                {
                    this.logger.Debug($"New approval created for employee {approvedById} for sick leave with id {existingSickLeave.Id}");

                    newApproval = new SickLeaveAccepts
                    {
                        At = timestamp,
                        ById = approvedById
                    };
                    existingSickLeave.SickLeaveAccepts.Add(newApproval);
                }

                context.SickLeaves.Update(existingSickLeave);

                await context.SaveChangesAsync();

                return newApproval != null
                    ? new Approval(newApproval.At, newApproval.ById.ToString())
                    : null;
            }
        }

        private Task<List<SickLeaves>> GetSickLeavesInternal(
            ArcadiaCspContext context,
            string employeeId = null,
            string calendarEventId = null,
            bool trackChanges = true)
        {
            int? employeeDbId = null;
            if (int.TryParse(employeeId, out var tempEmployeeId))
            {
                employeeDbId = tempEmployeeId;
            }

            var sickLeaveDbId = calendarEventId == null
                ? (int?)null
                : this.calendarEventIdParser.GetCspIdFromCalendarEvent(calendarEventId, CalendarEventTypes.Sickleave);

            var sickLeaves = context.SickLeaves
                .Include(v => v.SickLeaveAccepts)
                .Include(v => v.SickLeaveCancellations)
                .Include(v => v.SickLeaveCompletes)
                .Include(v => v.SickLeaveRejects)
                .Where(v => (employeeId == null || v.EmployeeId == employeeDbId) && (calendarEventId == null || v.Id == sickLeaveDbId));

            if (!trackChanges)
            {
                sickLeaves = sickLeaves.AsNoTracking();
            }

            return sickLeaves.ToListAsync();
        }

        private void EnsureSickLeaveIsNotCompleted(SickLeaves sickLeave)
        {
            if (sickLeave.SickLeaveCompletes.Any())
            {
                throw new Exception($"Sick leave with id {sickLeave.Id} have been already completed and cannot be changed");
            }
        }

        private SickLeaves CreateSickLeaveFromCalendarEvent(
            CalendarEvent @event,
            DateTimeOffset timestamp,
            string updatedBy)
        {
            var sickLeave = new SickLeaves
            {
                EmployeeId = int.Parse(@event.EmployeeId),
                RaisedAt = timestamp,
                Start = @event.Dates.StartDate.Date,
                End = @event.Dates.EndDate.Date
            };

            var updatedById = int.Parse(updatedBy);

            if (@event.Status == SickLeaveStatuses.Cancelled)
            {
                var sickLeaveCancellation = new SickLeaveCancellations
                {
                    At = timestamp,
                    ById = updatedById
                };
                sickLeave.SickLeaveCancellations.Add(sickLeaveCancellation);
            }
            else if (@event.Status == SickLeaveStatuses.Rejected)
            {
                var sickLeaveRejection = new SickLeaveRejects
                {
                    At = timestamp,
                    ById = updatedById
                };
                sickLeave.SickLeaveRejects.Add(sickLeaveRejection);
            }
            else if (@event.Status == SickLeaveStatuses.Approved)
            {
                var sickLeaveApproval = new SickLeaveAccepts
                {
                    ById = updatedById,
                    At = timestamp
                };
                sickLeave.SickLeaveAccepts.Add(sickLeaveApproval);
            }

            return sickLeave;
        }

        private CalendarEventWithAdditionalData CreateCalendarEventFromSickLeave(SickLeaves sickLeave)
        {
            var completed = sickLeave.SickLeaveCompletes
                .Select(c => new CalendarEventWithAdditionalData.SickLeaveCompletion(c.ById.ToString(), c.At))
                .FirstOrDefault();

            var cancelled = sickLeave.SickLeaveCancellations
                .Select(c => new CalendarEventWithAdditionalData.SickLeaveCancellation(c.ById.ToString(), c.At))
                .FirstOrDefault();

            var rejected = sickLeave.SickLeaveRejects
                .Select(r => new CalendarEventWithAdditionalData.SickLeaveRejection(r.ById.ToString(), r.At))
                .FirstOrDefault();

            var approved = sickLeave.SickLeaveAccepts
                .FirstOrDefault();

            var statuses = new[]
            {
                Tuple.Create(completed?.Timestamp, SickLeaveStatuses.Completed),
                Tuple.Create(cancelled?.Timestamp, SickLeaveStatuses.Cancelled),
                Tuple.Create(rejected?.Timestamp, SickLeaveStatuses.Rejected),
                Tuple.Create(approved?.At, SickLeaveStatuses.Approved)
            };

            var status = statuses
                .Where(x => x.Item1 != null)
                .OrderByDescending(x => x.Item1)
                .FirstOrDefault()
                ?.Item2;
            status = status ?? SickLeaveStatuses.Requested;

            var calendarEvent = new CalendarEvent(
                this.calendarEventIdParser.GetCalendarEventIdFromCspId(sickLeave.Id, CalendarEventTypes.Sickleave),
                CalendarEventTypes.Sickleave,
                new DatesPeriod(sickLeave.Start.Date, sickLeave.End.Date),
                status,
                sickLeave.EmployeeId.ToString());

            var approvals = sickLeave.SickLeaveAccepts
                .Select(a => new Approval(a.At, a.ById.ToString()))
                .ToList();

            return new CalendarEventWithAdditionalData(calendarEvent, approvals, cancelled, rejected, completed);
        }
    }
}