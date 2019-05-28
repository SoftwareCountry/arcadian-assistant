namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class CspSickLeavesRegistry : UntypedActor, ILogReceive
    {
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals";

        private readonly SickLeavesSyncExecutor sickLeavesSyncExecutor;
        private readonly IRefreshInformation refreshInformation;
        private readonly ActorSelection calendarEventsApprovalsChecker;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private ICancelable databaseRefreshSchedule;
        private DatabaseSickLeavesCache databaseSickLeavesCache;

        public CspSickLeavesRegistry(
            SickLeavesSyncExecutor sickLeavesSyncExecutor,
            IRefreshInformation refreshInformation)
        {
            this.sickLeavesSyncExecutor = sickLeavesSyncExecutor;
            this.refreshInformation = refreshInformation;
            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            // CRUTCH, CRUTCH, CRUUUUUUUUTCH
            // Events recovery complete message was broadcast before an actor that should have been receiving them was created.
            // So, 30 seconds delay is added to fix this.
            Context.System.Scheduler.ScheduleTellOnce(
                TimeSpan.FromSeconds(30),
                this.Self,
                Initialize.Instance,
                this.Self);
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<CspSickLeavesRegistry>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Initialize _:
                    this.GetSickLeaves(false)
                        .PipeTo(
                            this.Self,
                            success: result => new Initialize.Success(result),
                            failure: err => new Initialize.Error(err));
                    break;

                case Initialize.Success msg:
                    var eventsById = msg.Events.ToDictionary(x => x.CalendarEvent.EventId);
                    this.databaseSickLeavesCache = new DatabaseSickLeavesCache(eventsById);

                    foreach (var @event in msg.Events.Where(e => this.IsCalendarEventActual(e.CalendarEvent)))
                    {
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event.CalendarEvent));
                    }

                    this.ScheduleNextDatabaseRefresh();

                    break;

                case Initialize.Error msg:
                    this.logger.Error(msg.Exception, "Error occured on sick leaves recover");

                    Context.System.Scheduler.ScheduleTellOnce(
                        TimeSpan.FromMinutes(this.refreshInformation.IntervalInMinutes),
                        this.Self,
                        Initialize.Instance,
                        this.Self);

                    break;

                case RefreshDatabase _:
                    this.GetSickLeaves(false)
                        .PipeTo(
                            this.Self,
                            success: result => new RefreshDatabase.Success(result),
                            failure: err => new RefreshDatabase.Error(err));
                    break;

                case RefreshDatabase.Success msg:
                    this.UpdateDatabaseSickLeavesCache(msg.Events.ToList());
                    this.ScheduleNextDatabaseRefresh();
                    break;

                case RefreshDatabase.Error msg:
                    this.logger.Error(msg.Exception, "Error occured on refresh sick leaves from CSP database");
                    this.ScheduleNextDatabaseRefresh();
                    break;

                case RefreshDatabaseCreatedSuccess msg:
                    this.logger.Debug($"Sick leave with id {msg.Event.EventId} for employee {msg.Event.EmployeeId} " +
                        "was added to CSP database manually");

                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Event, msg.CreatedBy, msg.Timestamp));

                    if (msg.NextApprover != null)
                    {
                        this.logger.Debug($"Next event approver is {msg.NextApprover}. Event is pending and will be added to pending actions.");
                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }
                    else
                    {
                        this.logger.Debug("There is no next event approver, event is not pending and won't be added to pending actions.");
                    }

                    break;

                case RefreshDatabaseApprovalsUpdatedSuccess msg:
                    this.logger.Debug(
                        $"Approvals for sick leave with id {msg.Event.EventId} for employee {msg.Event.EmployeeId} " +
                        "were updated in CSP database manually");

                    Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(msg.Event, msg.Approvals));

                    if (msg.NextApprover != null)
                    {
                        this.logger.Debug($"Next event approver is {msg.NextApprover}. Event is pending and will be added to pending actions.");
                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApprover));
                        Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApprover));
                    }
                    else
                    {
                        this.logger.Debug("There is no next event approver, event is not pending and will be removed from current approver pending actions.");
                        Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(msg.Event));
                    }

                    break;

                case RefreshDatabaseUpdateCache msg:
                    this.databaseSickLeavesCache.Update(msg.Diff);
                    break;

                case GetEmployeeCalendarEvents msg:
                    this.GetSickLeaves(false)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new GetEmployeeCalendarEventsSuccess(msg.EmployeeId, result),
                            err => new GetEmployeeCalendarEventsError(msg.EmployeeId, err));
                    break;

                case GetEmployeeCalendarEventsSuccess msg:
                    this.FinishGetCalendarEvents(msg);
                    break;

                case GetEmployeeCalendarEventsError msg:
                    this.logger.Error(msg.Exception, $"Error occured on get sick leaves from CSP database for employee {msg.EmployeeId}");
                    break;

                case GetEmployeeCalendarEvent msg:
                    this.GetSickLeave(msg.EmployeeId, msg.EventId)
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
                    this.GetSickLeave(msg.Event.EmployeeId, msg.Event.EventId)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    return new GetCalendarEventApprovals.SuccessResponse(result.Approvals.ToList());
                                }

                                return new GetCalendarEventApprovals.ErrorResponse($"Sick leave with id {msg.Event.EventId} is not found");
                            },
                            failure: err => new GetCalendarEventApprovals.ErrorResponse(err.ToString()));
                    break;

                case InsertSickLeave msg:
                    this.sickLeavesSyncExecutor.InsertSickLeave(msg.Event, msg.Timestamp, msg.CreatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                this.databaseSickLeavesCache[result.CalendarEvent.EventId] = result;
                                return new InsertSickLeave.Success(result.CalendarEvent, msg.CreatedBy, msg.Timestamp);
                            },
                            failure: err => new InsertSickLeave.Error(err));
                    break;

                case UpdateSickLeave msg:
                    this.sickLeavesSyncExecutor.UpdateSickLeave(msg.NewEvent, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                this.databaseSickLeavesCache[result.CalendarEvent.EventId] = result;
                                return new UpdateSickLeave.Success(result.CalendarEvent, msg.OldEvent, msg.UpdatedBy, msg.Timestamp);
                            },
                            failure: err => new UpdateSickLeave.Error(err));
                    break;

                case ApproveSickLeave msg:
                    this.ApproveSickLeave(msg)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    this.databaseSickLeavesCache[result.CalendarEvent.EventId] = result;

                                    return new ApproveSickLeave.Success(
                                        result.CalendarEvent,
                                        result.Approvals.ToList(),
                                        msg.ApprovedBy,
                                        msg.Timestamp);
                                }

                                return Calendar.Abstractions.EmployeeSickLeaves.ApproveSickLeave.Success.Instance;
                            },
                            failure: err => new ApproveSickLeave.Error(err));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void FinishGetCalendarEvents(GetEmployeeCalendarEventsSuccess msg)
        {
            var employeeSickLeaves = msg.Events
                .Where(e => e.CalendarEvent.EmployeeId == msg.EmployeeId && this.IsCalendarEventActual(e.CalendarEvent))
                .Select(x => x.CalendarEvent)
                .ToList();
            this.Sender.Tell(new GetCalendarEvents.Response(msg.EmployeeId, employeeSickLeaves));

            this.UpdateDatabaseSickLeavesCache(msg.Events.ToList());

            this.ScheduleNextDatabaseRefresh();
        }

        private void UpdateDatabaseSickLeavesCache(IEnumerable<CalendarEventWithAdditionalData> databaseSickLeaves)
        {
            var databaseSickLeavesById = databaseSickLeaves.ToDictionary(x => x.CalendarEvent.EventId);

            var diff = this.databaseSickLeavesCache.Difference(databaseSickLeavesById);

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

                // If sick leave has entirely processed its flow in the database and has same status differs from initial,
                // we imply, for simplicity, that it was changed from the same sick leave, but with initial status
                var oldEvent =
                    this.databaseSickLeavesCache[@event.CalendarEvent.EventId]?.CalendarEvent ??
                    new CalendarEvent(
                        newEvent.EventId,
                        newEvent.Type,
                        newEvent.Dates,
                        SickLeaveStatuses.Requested,
                        newEvent.EmployeeId);

                var updatedBy = newEvent.EmployeeId;
                var timestamp = DateTimeOffset.Now;

                if (oldEvent.Status != newEvent.Status)
                {
                    switch (newEvent.Status)
                    {
                        case SickLeaveStatuses.Approved:

                            var lastApproval = @event.Approvals
                                .OrderByDescending(x => x.Timestamp)
                                .First();

                            updatedBy = lastApproval.ApprovedBy;
                            timestamp = lastApproval.Timestamp;

                            break;

                        case SickLeaveStatuses.Rejected:
                            updatedBy = @event.Rejected.RejectedBy;
                            timestamp = @event.Rejected.Timestamp;
                            break;

                        case SickLeaveStatuses.Completed:
                            updatedBy = @event.Completed.CompletedBy;
                            timestamp = @event.Completed.Timestamp;
                            break;

                        case SickLeaveStatuses.Cancelled:
                            updatedBy = @event.Cancelled.CancelledBy;
                            timestamp = @event.Cancelled.Timestamp;
                            break;
                    }
                }

                this.logger.Debug($"Sick leave with id {newEvent.EventId} for employee {newEvent.EmployeeId} was updated in CSP database manually");

                Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, updatedBy, timestamp, newEvent));

                if (!newEvent.IsPending)
                {
                    this.logger.Debug("Event is not pending and will be removed from current approver pending actions.");
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
                this.logger.Debug(
                    $"Sick leave with id {@event.CalendarEvent.EventId} for employee " +
                    $"{@event.CalendarEvent.EmployeeId} was removed from CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventRemoved(@event.CalendarEvent));
            }

            Task.WhenAll(updateCacheTasks)
                .PipeTo(
                    this.Self,
                    success: () => new RefreshDatabaseUpdateCache(diff));
        }

        private async Task<IEnumerable<CalendarEventWithAdditionalData>> GetSickLeaves(bool onlyActual = true)
        {
            var sickLeaves = await this.sickLeavesSyncExecutor.GetSickLeaves();

            if (onlyActual)
            {
                sickLeaves = sickLeaves.Where(v => this.IsCalendarEventActual(v.CalendarEvent)).ToList();
            }

            return sickLeaves;
        }

        private async Task<CalendarEventWithAdditionalData> GetSickLeave(string employeeId, string eventId)
        {
            var sickLeave = await this.sickLeavesSyncExecutor.GetSickLeave(employeeId, eventId);

            if (sickLeave == null)
            {
                return null;
            }

            return this.IsCalendarEventActual(sickLeave.CalendarEvent) ? sickLeave : null;
        }

        private async Task<CalendarEventWithAdditionalData> ApproveSickLeave(ApproveSickLeave message)
        {
            var sickLeave = await this.GetSickLeave(message.Event.EmployeeId, message.Event.EventId);

            if (sickLeave == null)
            {
                throw new Exception($"Sick leave with id {message.Event.EventId} is not found");
            }

            var calendarEvent = sickLeave.CalendarEvent;
            var approvals = sickLeave.Approvals;

            if (approvals.Any(a => a.ApprovedBy == message.ApprovedBy))
            {
                return null;
            }

            await this.sickLeavesSyncExecutor.UpsertSickLeaveApproval(calendarEvent, message.Timestamp, message.ApprovedBy);

            var newEvent = await this.GetSickLeave(calendarEvent.EmployeeId, calendarEvent.EventId);
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

        private bool IsCalendarEventActual(CalendarEvent @event)
        {
            return SickLeaveStatuses.Actual.Contains(@event.Status);
        }

        private void ScheduleNextDatabaseRefresh()
        {
            this.databaseRefreshSchedule.CancelIfNotNull();

            this.databaseRefreshSchedule = Context.System.Scheduler.ScheduleTellOnceCancelable(
                TimeSpan.FromMinutes(this.refreshInformation.IntervalInMinutes),
                this.Self,
                RefreshDatabase.Instance,
                this.Self);
        }

        public class GetEmployeeCalendarEvents
        {
            public GetEmployeeCalendarEvents(string employeeId)
            {
                this.EmployeeId = employeeId;
            }

            public string EmployeeId { get; }
        }

        public class GetEmployeeCalendarEvent
        {
            public GetEmployeeCalendarEvent(string employeeId, string eventId)
            {
                this.EmployeeId = employeeId;
                this.EventId = eventId;
            }

            public string EmployeeId { get; }

            public string EventId { get; }
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
            public RefreshDatabaseUpdateCache(DatabaseSickLeavesCache.Diff diff)
            {
                this.Diff = diff;
            }

            public DatabaseSickLeavesCache.Diff Diff { get; }
        }

        private class GetEmployeeCalendarEventsSuccess
        {
            public GetEmployeeCalendarEventsSuccess(string employeeId, IEnumerable<CalendarEventWithAdditionalData> events)
            {
                this.EmployeeId = employeeId;
                this.Events = events;
            }

            public IEnumerable<CalendarEventWithAdditionalData> Events { get; }

            public string EmployeeId { get; }
        }

        private class GetEmployeeCalendarEventsError
        {
            public GetEmployeeCalendarEventsError(string employeeId, Exception exception)
            {
                this.EmployeeId = employeeId;
                this.Exception = exception;
            }

            public Exception Exception { get; }

            public string EmployeeId { get; }
        }
    }
}