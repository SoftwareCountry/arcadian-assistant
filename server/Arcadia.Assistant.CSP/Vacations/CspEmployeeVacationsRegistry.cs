namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP.Configuration;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly IRefreshInformation refreshInformation;
        private readonly string employeeId;
        private readonly ActorSelection calendarEventsApprovalsChecker;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private ICancelable databaseRefreshSchedule;
        private DatabaseVacationsCache databaseVacationsCache;

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            AccountingReminderConfiguration reminderConfiguration,
            IRefreshInformation refreshInformation,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.refreshInformation = refreshInformation;
            this.employeeId = employeeId;
            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            // Not better place to create it, but I don't know where we can do it else
            Context.ActorOf(
                EmployeeVacationApprovedAccountingReminderActor.CreateProps(employeeId, reminderConfiguration),
                $"vacations-reminder-{employeeId}");

            this.Self.Tell(Initialize.Instance);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Initialize _:
                    this.GetVacations(false)
                        .PipeTo(
                            this.Self,
                            success: result => new Initialize.Success(result),
                            failure: err => new Initialize.Error(err));
                    break;

                case Initialize.Success msg:
                    var eventsById = msg.Events.ToDictionary(x => x.CalendarEvent.EventId);
                    this.databaseVacationsCache = new DatabaseVacationsCache(eventsById);

                    foreach (var @event in msg.Events.Where(e => this.IsCalendarEventActual(e.CalendarEvent)))
                    {
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event.CalendarEvent));
                    }

                    this.ScheduleNextDatabaseRefresh();

                    break;

                case Initialize.Error msg:
                    this.logger.Error(msg.Exception, $"Error occured on vacations recover for employee {this.employeeId}");
                    break;

                case RefreshDatabase _:
                    this.GetVacations(false)
                        .PipeTo(
                            this.Self,
                            success: result => new RefreshDatabase.Success(result),
                            failure: err => new RefreshDatabase.Error(err));
                    break;

                case RefreshDatabase.Success msg:
                    this.FinishDatabaseRefresh(msg);
                    break;

                case RefreshDatabase.Error msg:
                    this.logger.Error(msg.Exception, $"Error occured on refresh vacations from CSP database for employee {this.employeeId}");
                    break;

                case RefreshDatabaseCreatedSuccess msg:
                    this.logger.Debug($"Vacation with id {msg.Event.EventId} for employee {this.employeeId} was added to CSP database manually");

                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Event, msg.CreatedBy, msg.Timestamp));

                    if (msg.NextApprover != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }

                    break;

                case RefreshDatabaseApprovalsUpdatedSuccess msg:
                    this.logger.Debug(
                        $"Approvals for vacation with id {msg.Event.EventId} for employee {this.employeeId} " +
                        "were updated in CSP database manually");
                    Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(msg.Event, msg.Approvals));

                    if (msg.NextApprover != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }

                    break;

                case RefreshDatabaseUpdateCache msg:
                    this.databaseVacationsCache.Update(msg.Diff);
                    break;

                case GetCalendarEvents _:
                    this.GetVacations(false)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new GetCalendarEventsSuccess(result),
                            err => new GetCalendarEventsError(err));
                    break;

                case GetCalendarEventsSuccess msg:
                    this.FinishGetCalendarEvents(msg);
                    break;

                case GetCalendarEventsError msg:
                    this.logger.Error(msg.Exception, $"Error occured on get vacations from CSP database for employee {this.employeeId}");
                    break;

                case GetCalendarEvent msg:
                    this.GetVacation(msg.EventId)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    return new GetCalendarEvent.Response.Found(result.CalendarEvent);
                                }

                                return new GetCalendarEvent.Response.NotFound();
                            },
                            failure: err => new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEventApprovals msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    return new GetCalendarEventApprovals.SuccessResponse(result.Approvals.ToList());
                                }

                                return new GetCalendarEventApprovals.ErrorResponse($"Vacation with id {msg.Event.EventId} is not found");
                            },
                            failure: err => new GetCalendarEventApprovals.ErrorResponse(err.ToString()));
                    break;

                case InsertVacation msg:
                    this.vacationsSyncExecutor.InsertVacation(msg.Event, msg.Timestamp, msg.CreatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                this.databaseVacationsCache[result.CalendarEvent.EventId] = result;
                                return new InsertVacation.Success(result.CalendarEvent, msg.CreatedBy, msg.Timestamp);
                            },
                            failure: err => new InsertVacation.Error(err));
                    break;

                case UpdateVacation msg:
                    this.vacationsSyncExecutor.UpdateVacation(msg.NewEvent, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                this.databaseVacationsCache[result.CalendarEvent.EventId] = result;
                                return new UpdateVacation.Success(result.CalendarEvent, msg.OldEvent, msg.UpdatedBy, msg.Timestamp);
                            },
                            failure: err => new UpdateVacation.Error(err));
                    break;

                case ApproveVacation msg:
                    this.ApproveVacation(msg)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    this.databaseVacationsCache[result.CalendarEvent.EventId] = result;
                                    return new ApproveVacation.Success(
                                        result.CalendarEvent,
                                        result.Approvals.ToList(),
                                        msg.ApprovedBy,
                                        msg.Timestamp);
                                }

                                return Calendar.Abstractions.EmployeeVacations.ApproveVacation.Success.Instance;
                            },
                            failure: err => new ApproveVacation.Error(err));
                    break;

                case CheckDatesAvailability msg:
                    this.CheckDatesAvailability(msg.Event)
                        .PipeTo(
                            this.Sender,
                            success: result => new CheckDatesAvailability.Success(result),
                            failure: err => new CheckDatesAvailability.Error(err));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void FinishDatabaseRefresh(RefreshDatabase.Success msg)
        {
            this.UpdateDatabaseVacationsCache(msg.Events.ToList());
            this.ScheduleNextDatabaseRefresh();
        }

        private void FinishGetCalendarEvents(GetCalendarEventsSuccess msg)
        {
            var vacations = msg.Events
                .Where(e => this.IsCalendarEventActual(e.CalendarEvent))
                .Select(x => x.CalendarEvent)
                .ToList();
            this.Sender.Tell(new GetCalendarEvents.Response(this.employeeId, vacations));

            this.UpdateDatabaseVacationsCache(msg.Events.ToList());

            this.ScheduleNextDatabaseRefresh();
        }

        private void UpdateDatabaseVacationsCache(List<CalendarEventWithAdditionalData> databaseVacations)
        {
            var databaseVacationsById = databaseVacations.ToDictionary(x => x.CalendarEvent.EventId);

            var diff = this.databaseVacationsCache.Difference(databaseVacationsById);

            var updateCacheTasks = new List<Task>();

            foreach (var @event in diff.Created)
            {
                var task = this.GetNextApproverId(@event.CalendarEvent, @event.Approvals);
                updateCacheTasks.Add(task);

                task.PipeTo(
                    this.Self,
                    success: result => new RefreshDatabaseCreatedSuccess(
                        @event.CalendarEvent,
                        @event.CalendarEvent.EmployeeId,
                        DateTimeOffset.Now,
                        result),
                    failure: err => new RefreshDatabase.Error(err));
            }

            foreach (var @event in diff.Updated)
            {
                var newEvent = @event.CalendarEvent;

                var oldEvent = this.databaseVacationsCache[@event.CalendarEvent.EventId]?.CalendarEvent;
                if (oldEvent == null)
                {
                    // If vacation has entirely processed its flow in the database and has same status differs from initial,
                    // we imply, for simplicity, that it was changed from the same vacation, but with initial status
                    oldEvent = new CalendarEvent(
                        newEvent.EventId,
                        newEvent.Type,
                        newEvent.Dates,
                        VacationStatuses.Requested,
                        newEvent.EmployeeId);
                }

                string updatedBy = this.employeeId;
                DateTimeOffset timestamp = DateTimeOffset.Now;

                if (oldEvent.Status != newEvent.Status)
                {
                    switch (newEvent.Status)
                    {
                        case VacationStatuses.Approved:
                            var lastApproval = @event.Approvals
                                .OrderByDescending(x => x.Timestamp)
                                .First();

                            updatedBy = lastApproval.ApprovedBy;
                            timestamp = lastApproval.Timestamp;

                            break;

                        case VacationStatuses.Rejected:
                            updatedBy = @event.Rejected.RejectedBy;
                            timestamp = @event.Rejected.Timestamp;
                            break;

                        case VacationStatuses.Processed:
                            updatedBy = @event.Processed.ProcessedBy;
                            timestamp = @event.Processed.Timestamp;
                            break;

                        case VacationStatuses.Cancelled:
                            updatedBy = @event.Cancelled.CancelledBy;
                            timestamp = @event.Cancelled.Timestamp;
                            break;
                    }
                }

                this.logger.Debug($"Vacation with id {newEvent.EventId} for employee {this.employeeId} was updated in CSP database manually");

                Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, updatedBy, timestamp, newEvent));

                if (!newEvent.IsPending)
                {
                    Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(newEvent));
                }
            }

            foreach (var @event in diff.ApprovalsUpdated)
            {
                var task = this.GetNextApproverId(@event.CalendarEvent, @event.Approvals);
                updateCacheTasks.Add(task);

                task.PipeTo(
                    this.Self,
                    success: result => new RefreshDatabaseApprovalsUpdatedSuccess(@event.CalendarEvent, @event.Approvals, result),
                    failure: err => new RefreshDatabase.Error(err));
            }

            foreach (var @event in diff.Removed)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} for employee {this.employeeId} was removed from CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventRemoved(@event.CalendarEvent));
            }

            Task.WhenAll(updateCacheTasks)
                .PipeTo(
                    this.Self,
                    success: () => new RefreshDatabaseUpdateCache(diff));
        }

        private async Task<IEnumerable<CalendarEventWithAdditionalData>> GetVacations(bool onlyActual = true)
        {
            var vacations = await this.vacationsSyncExecutor.GetVacations(this.employeeId);

            if (onlyActual)
            {
                vacations = vacations.Where(v => this.IsCalendarEventActual(v.CalendarEvent)).ToList();
            }

            return vacations;
        }

        private async Task<CalendarEventWithAdditionalData> GetVacation(string eventId)
        {
            var vacation = await this.vacationsSyncExecutor.GetVacation(this.employeeId, eventId);

            if (vacation == null)
            {
                return null;
            }

            return this.IsCalendarEventActual(vacation.CalendarEvent) ? vacation : null;
        }

        private async Task<CalendarEventWithAdditionalData> ApproveVacation(ApproveVacation message)
        {
            var vacation = await this.GetVacation(message.Event.EventId);
            if (vacation == null)
            {
                throw new Exception($"Vacation with id {message.Event.EventId} is not found");
            }

            var calendarEvent = vacation.CalendarEvent;
            var approvals = vacation.Approvals;

            if (approvals.Any(a => a.ApprovedBy == message.ApprovedBy))
            {
                return null;
            }

            await this.vacationsSyncExecutor.UpsertVacationApproval(calendarEvent, message.Timestamp, message.ApprovedBy);

            var newEvent = await this.GetVacation(calendarEvent.EventId);
            return newEvent;
        }

        private async Task<string> GetNextApproverId(CalendarEvent @event, IEnumerable<Approval> approvals)
        {
            var approvedBy = approvals.Select(a => a.ApprovedBy);

            var response = await this.calendarEventsApprovalsChecker
                .Ask<GetNextCalendarEventApprover.Response>(
                    new GetNextCalendarEventApprover(@event.EmployeeId, approvedBy, @event.Type));

            switch (response)
            {
                case GetNextCalendarEventApprover.SuccessResponse msg:
                    return msg.NextApproverEmployeeId;

                case GetNextCalendarEventApprover.ErrorResponse msg:
                    throw new Exception(msg.Message);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<bool> CheckDatesAvailability(CalendarEvent @event)
        {
            var vacations = await this.GetVacations();

            var intersectedEventExists = vacations
                .Where(v => v.CalendarEvent.EventId != @event.EventId)
                .Any(v => v.CalendarEvent.Dates.DatesIntersectsWith(@event.Dates));
            return !intersectedEventExists;
        }

        private bool IsCalendarEventActual(CalendarEvent @event)
        {
            return VacationStatuses.Actual.Contains(@event.Status);
        }

        private void ScheduleNextDatabaseRefresh()
        {
            this.databaseRefreshSchedule.CancelIfNotNull();

            this.databaseRefreshSchedule = Context.System.Scheduler.ScheduleTellOnceCancelable(
                //TimeSpan.FromMinutes(this.refreshInformation.IntervalInMinutes),
                TimeSpan.FromMinutes(1),
                this.Self,
                RefreshDatabase.Instance,
                this.Self);
        }

        private class Initialize
        {
            public static readonly Initialize Instance = new Initialize();

            public class Success
            {
                public Success(IEnumerable<CalendarEventWithAdditionalData> events)
                {
                    this.Events = events;
                }

                public IEnumerable<CalendarEventWithAdditionalData> Events { get; }
            }

            public class Error
            {
                public Error(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }

        private class RefreshDatabase
        {
            public static readonly RefreshDatabase Instance = new RefreshDatabase();

            public class Success
            {
                public Success(IEnumerable<CalendarEventWithAdditionalData> events)
                {
                    this.Events = events;
                }

                public IEnumerable<CalendarEventWithAdditionalData> Events { get; }
            }

            public class Error
            {
                public Error(Exception exception)
                {
                    this.Exception = exception;
                }

                public Exception Exception { get; }
            }
        }

        private class RefreshDatabaseCreatedSuccess
        {
            public RefreshDatabaseCreatedSuccess(
                CalendarEvent @event,
                string createdBy,
                DateTimeOffset timestamp,
                string nextApprover)
            {
                this.Event = @event;
                this.CreatedBy = createdBy;
                this.Timestamp = timestamp;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public string CreatedBy { get; }

            public DateTimeOffset Timestamp { get; }

            public string NextApprover { get; }
        }

        private class RefreshDatabaseApprovalsUpdatedSuccess
        {
            public RefreshDatabaseApprovalsUpdatedSuccess(
                CalendarEvent @event,
                IEnumerable<Approval> approvals,
                string nextApprover)
            {
                this.Event = @event;
                this.Approvals = approvals;
                this.NextApprover = nextApprover;
            }

            public CalendarEvent Event { get; }

            public IEnumerable<Approval> Approvals { get; }

            public string NextApprover { get; }
        }

        private class RefreshDatabaseUpdateCache
        {
            public RefreshDatabaseUpdateCache(DatabaseVacationsCache.Diff diff)
            {
                this.Diff = diff;
            }

            public DatabaseVacationsCache.Diff Diff { get; }
        }

        private class GetCalendarEventsSuccess
        {
            public GetCalendarEventsSuccess(IEnumerable<CalendarEventWithAdditionalData> events)
            {
                this.Events = events;
            }

            public IEnumerable<CalendarEventWithAdditionalData> Events { get; }
        }

        private class GetCalendarEventsError
        {
            public GetCalendarEventsError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}