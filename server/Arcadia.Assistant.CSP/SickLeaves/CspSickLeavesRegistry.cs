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

    public class CspSickLeavesRegistry : UntypedActor, ILogReceive
    {
        private readonly SickLeavesSyncExecutor sickLeavesSyncExecutor;
        private readonly IRefreshInformation refreshInformation;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private ICancelable databaseRefreshSchedule;
        private DatabaseSickLeavesCache databaseSickLeavesCache;

        public CspSickLeavesRegistry(
            SickLeavesSyncExecutor sickLeavesSyncExecutor,
            IRefreshInformation refreshInformation)
        {
            this.sickLeavesSyncExecutor = sickLeavesSyncExecutor;
            this.refreshInformation = refreshInformation;

            this.Self.Tell(Initialize.Instance);
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

            foreach (var @event in diff.Created)
            {
                var newEvent = @event.CalendarEvent;
                var timestamp = DateTimeOffset.Now;

                this.logger.Debug($"Sick leave with id {newEvent.EventId} for employee {newEvent.EmployeeId} " +
                    "was added to CSP database manually");

                Context.System.EventStream.Publish(new CalendarEventCreated(newEvent, newEvent.EmployeeId, timestamp));
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
            }

            foreach (var @event in diff.Removed)
            {
                this.logger.Debug(
                    $"Sick leave with id {@event.CalendarEvent.EventId} for employee " +
                    $"{@event.CalendarEvent.EmployeeId} was removed from CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventRemoved(@event.CalendarEvent));
            }

            this.databaseSickLeavesCache.Update(diff);
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