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

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly IRefreshInformation refreshInformation;
        private readonly string employeeId;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private ICancelable databaseRefreshSchedule;
        private Dictionary<string, CalendarEventWithApprovals> databaseVacationsCache;

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            AccountingReminderConfiguration reminderConfiguration,
            IRefreshInformation refreshInformation,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.refreshInformation = refreshInformation;
            this.employeeId = employeeId;

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
                    this.GetVacations()
                        .PipeTo(
                            this.Self,
                            success: result => new Initialize.Success(result),
                            failure: err => new Initialize.Error(err));
                    break;

                case Initialize.Success msg:
                    this.databaseVacationsCache = msg.Events.ToDictionary(x => x.CalendarEvent.EventId);

                    foreach (var @event in msg.Events)
                    {
                        Context.System.EventStream.Publish(new CalendarEventRecoverComplete(@event.CalendarEvent));
                    }

                    this.ScheduleNextDatabaseRefresh();

                    break;

                case Initialize.Error msg:
                    this.logger.Error(msg.Exception, $"Error occured on vacations recover for employee {this.employeeId}");
                    break;

                case RefreshDatabase _:
                    this.GetVacations()
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

                case GetCalendarEvents _:
                    this.GetVacations()
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
                            this.Self,
                            this.Sender,
                            result => new GetCalendarEventSuccess(msg.EventId, result),
                            err => new GetCalendarEventError(msg.EventId, err));
                    break;

                case GetCalendarEventSuccess msg:
                    this.FinishGetCalendarEvent(msg);
                    break;

                case GetCalendarEventError msg:
                    this.logger.Error(msg.Exception, $"Error occured on get vacation with id {msg.EventId} from CSP database for employee {this.employeeId}");
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEventApprovals msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new GetCalendarEventApprovalsSuccess(msg.Event.EventId, result),
                            err => new GetCalendarEventApprovalsError(msg.Event.EventId, err));
                    break;

                case GetCalendarEventApprovalsSuccess msg:
                    this.FinishGetCalendarEventApprovals(msg);
                    break;

                case GetCalendarEventApprovalsError msg:
                    this.logger.Error(msg.Exception, $"Error occured on get approvals for vacation with id {msg.EventId} from CSP database for employee {this.employeeId}");
                    this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse(msg.Exception.ToString()));
                    break;

                case InsertVacation msg:
                    this.vacationsSyncExecutor.InsertVacation(msg.Event, msg.Timestamp, msg.CreatedBy)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new InsertVacationSuccess(result, msg.CreatedBy, msg.Timestamp),
                            failure: err => new InsertVacationError(err));
                    break;

                case InsertVacationSuccess msg:
                    this.FinishInsertVacation(msg);
                    break;

                case InsertVacationError msg:
                    this.logger.Error(msg.Exception, $"Error occured on insert vacation to CSP database for employee {this.employeeId}");
                    this.Sender.Tell(new InsertVacation.Error(msg.Exception));
                    break;

                case UpdateVacation msg:
                    this.vacationsSyncExecutor.UpdateVacation(msg.NewEvent, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new UpdateVacationSuccess(result, msg.OldEvent, msg.UpdatedBy, msg.Timestamp),
                            err => new UpdateVacationError(err));
                    break;

                case UpdateVacationSuccess msg:
                    this.FinishUpdateVacation(msg);
                    break;

                case UpdateVacationError msg:
                    this.logger.Error(msg.Exception, $"Error occured on update vacation in CSP database for employee {this.employeeId}");
                    this.Sender.Tell(new UpdateVacation.Error(msg.Exception));
                    break;

                case ApproveVacation msg:
                    this.ApproveVacation(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new ApproveVacationSuccess(result, msg.ApprovedBy, msg.Timestamp),
                            err => new ApproveVacationError(err));
                    break;

                case ApproveVacationSuccess msg:
                    this.FinishApproveVacation(msg);
                    break;

                case ApproveVacationError msg:
                    this.logger.Error(msg.Exception, $"Error occured on update vacation in CSP database for employee {this.employeeId}");
                    this.Sender.Tell(new ApproveVacation.Error(msg.Exception));
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
            var vacations = msg.Events.Select(x => x.CalendarEvent).ToList();
            this.Sender.Tell(new GetCalendarEvents.Response(this.employeeId, vacations));

            this.UpdateDatabaseVacationsCache(msg.Events.ToList());

            this.ScheduleNextDatabaseRefresh();
        }

        private void FinishGetCalendarEvent(GetCalendarEventSuccess msg)
        {
            if (msg.Event == null)
            {
                this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                this.RemoveEventFromDatabaseCache(msg.EventId);
            }
            else
            {
                this.Sender.Tell(new GetCalendarEvent.Response.Found(msg.Event.CalendarEvent));
                this.UpdateEventInDatabaseCache(msg.Event);
            }
        }

        private void FinishGetCalendarEventApprovals(GetCalendarEventApprovalsSuccess msg)
        {
            if (msg.Event == null)
            {
                this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse($"Vacation with id {msg.EventId} is not found"));
                this.RemoveEventFromDatabaseCache(msg.EventId);
            }
            else
            {
                this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(msg.Event.Approvals.ToList()));
                this.UpdateEventInDatabaseCache(msg.Event);
            }
        }

        private void FinishInsertVacation(InsertVacationSuccess msg)
        {
            this.Sender.Tell(new InsertVacation.Success(msg.Event.CalendarEvent, msg.CreatedBy, msg.Timestamp));
            this.databaseVacationsCache[msg.Event.CalendarEvent.EventId] = msg.Event;
        }

        private void FinishUpdateVacation(UpdateVacationSuccess msg)
        {
            this.Sender.Tell(new UpdateVacation.Success(msg.Event.CalendarEvent, msg.OldEvent, msg.UpdatedBy, msg.Timestamp));
            this.databaseVacationsCache[msg.OldEvent.EventId] = msg.Event;
        }

        private void FinishApproveVacation(ApproveVacationSuccess msg)
        {
            if (msg.Event != null)
            {
                this.Sender.Tell(new ApproveVacation.Success(
                    msg.Event.CalendarEvent,
                    msg.Event.Approvals.ToList(),
                    msg.ApprovedBy,
                    msg.Timestamp)
                );

                this.databaseVacationsCache[msg.Event.CalendarEvent.EventId] = msg.Event;
            }
            else
            {
                this.Sender.Tell(Calendar.Abstractions.EmployeeVacations.ApproveVacation.Success.Instance);
            }
        }

        private void RemoveEventFromDatabaseCache(string eventId)
        {
            var newDatabaseVacations = this.databaseVacationsCache.Values
                .Where(x => x.CalendarEvent.EventId != eventId)
                .ToList();

            this.UpdateDatabaseVacationsCache(newDatabaseVacations);
        }

        private void UpdateEventInDatabaseCache(CalendarEventWithApprovals @event)
        {
            var newDatabaseVacations = this.databaseVacationsCache.Values
                .Where(x => x.CalendarEvent.EventId != @event.CalendarEvent.EventId)
                .ToList();
            newDatabaseVacations.Add(@event);

            this.UpdateDatabaseVacationsCache(newDatabaseVacations);
        }

        private void UpdateDatabaseVacationsCache(List<CalendarEventWithApprovals> databaseVacations)
        {
            var createdEvents = new List<CalendarEventWithApprovals>();
            var updatedEvents = new List<CalendarEventWithApprovals>();
            var approvalsUpdatedEvents = new List<CalendarEventWithApprovals>();

            foreach (var @event in databaseVacations)
            {
                if (!this.databaseVacationsCache.ContainsKey(@event.CalendarEvent.EventId))
                {
                    if (@event.CalendarEvent.Status == VacationStatuses.Requested)
                    {
                        if (!@event.Approvals.Any())
                        {
                            createdEvents.Add(@event);
                        }
                        else
                        {
                            approvalsUpdatedEvents.Add(@event);
                        }
                    }
                    else
                    {
                        updatedEvents.Add(@event);
                    }
                }
                else
                {
                    var cacheEvent = this.databaseVacationsCache[@event.CalendarEvent.EventId];

                    if (cacheEvent.CalendarEvent.Status != @event.CalendarEvent.Status || cacheEvent.CalendarEvent.Dates != @event.CalendarEvent.Dates)
                    {
                        updatedEvents.Add(@event);
                    }

                    if (@event.CalendarEvent.Status == VacationStatuses.Requested)
                    {
                        var cacheApprovals = cacheEvent.Approvals.Select(x => x.ApprovedBy).ToList();
                        var databaseApprovals = @event.Approvals.Select(x => x.ApprovedBy);

                        if (cacheApprovals.Intersect(databaseApprovals).Count() != cacheApprovals.Count)
                        {
                            approvalsUpdatedEvents.Add(@event);
                        }
                    }
                }
            }

            var removedEvents = this.databaseVacationsCache
                .Where(x => databaseVacations.All(e => e.CalendarEvent.EventId != x.Key))
                .Select(x => x.Value);

            foreach (var @event in createdEvents)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} was added to CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventCreated(@event.CalendarEvent, @event.CalendarEvent.EmployeeId, DateTimeOffset.Now));
                this.databaseVacationsCache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in updatedEvents)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} was updated in CSP database manually");
                var oldEvent = this.databaseVacationsCache[@event.CalendarEvent.EventId];
                Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent.CalendarEvent, @event.CalendarEvent.EmployeeId, DateTimeOffset.Now, @event.CalendarEvent));

                this.databaseVacationsCache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in approvalsUpdatedEvents)
            {
                this.logger.Debug($"Approvals for vacation with id {@event.CalendarEvent.EventId} was updated in CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(@event.CalendarEvent, @event.Approvals));
                this.databaseVacationsCache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in removedEvents)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} was removed from CSP database manually");
                // New event will be published here
            }
        }

        private async Task<IEnumerable<CalendarEventWithApprovals>> GetVacations()
        {
            var vacations = await this.vacationsSyncExecutor.GetVacations(this.employeeId);
            return vacations
                .Where(v => this.IsCalendarEventActual(v.CalendarEvent))
                .ToList();
        }

        private Task<CalendarEventWithApprovals> GetVacation(string eventId)
        {
            return this.vacationsSyncExecutor.GetVacation(this.employeeId, eventId);
        }

        private async Task<CalendarEventWithApprovals> ApproveVacation(ApproveVacation message)
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
                TimeSpan.FromMinutes(this.refreshInformation.IntervalInMinutes),
                this.Self,
                RefreshDatabase.Instance,
                this.Self);
        }

        private class Initialize
        {
            public static readonly Initialize Instance = new Initialize();

            public class Success
            {
                public Success(IEnumerable<CalendarEventWithApprovals> events)
                {
                    this.Events = events;
                }

                public IEnumerable<CalendarEventWithApprovals> Events { get; }
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
                public Success(IEnumerable<CalendarEventWithApprovals> events)
                {
                    this.Events = events;
                }

                public IEnumerable<CalendarEventWithApprovals> Events { get; }
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

        private class GetCalendarEventsSuccess
        {
            public GetCalendarEventsSuccess(IEnumerable<CalendarEventWithApprovals> events)
            {
                this.Events = events;
            }

            public IEnumerable<CalendarEventWithApprovals> Events { get; }
        }

        private class GetCalendarEventsError
        {
            public GetCalendarEventsError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }

        private class GetCalendarEventSuccess
        {
            public GetCalendarEventSuccess(string eventId, CalendarEventWithApprovals @event)
            {
                this.EventId = eventId;
                this.Event = @event;
            }

            public string EventId { get; }

            public CalendarEventWithApprovals Event { get; }
        }

        private class GetCalendarEventError
        {
            public GetCalendarEventError(string eventId, Exception exception)
            {
                this.EventId = eventId;
                this.Exception = exception;
            }

            public string EventId { get; }

            public Exception Exception { get; }
        }

        private class GetCalendarEventApprovalsSuccess
        {
            public GetCalendarEventApprovalsSuccess(string eventId, CalendarEventWithApprovals @event)
            {
                this.EventId = eventId;
                this.Event = @event;
            }

            public string EventId { get; }

            public CalendarEventWithApprovals Event { get; }
        }

        private class GetCalendarEventApprovalsError
        {
            public GetCalendarEventApprovalsError(string eventId, Exception exception)
            {
                this.EventId = eventId;
                this.Exception = exception;
            }

            public string EventId { get; }

            public Exception Exception { get; }
        }

        private class InsertVacationSuccess
        {
            public InsertVacationSuccess(CalendarEventWithApprovals @event, string createdBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.CreatedBy = createdBy;
                this.Timestamp = timestamp;
            }

            public CalendarEventWithApprovals Event { get; }

            public string CreatedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        private class InsertVacationError
        {
            public InsertVacationError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }

        private class UpdateVacationSuccess
        {
            public UpdateVacationSuccess(CalendarEventWithApprovals @event, CalendarEvent oldEvent, string updatedBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.OldEvent = oldEvent;
                this.UpdatedBy = updatedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEventWithApprovals Event { get; }

            public CalendarEvent OldEvent { get; }

            public string UpdatedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        private class UpdateVacationError
        {
            public UpdateVacationError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }

        private class ApproveVacationSuccess
        {
            public ApproveVacationSuccess(CalendarEventWithApprovals @event, string approvedBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.ApprovedBy = approvedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEventWithApprovals Event { get; }

            public string ApprovedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        private class ApproveVacationError
        {
            public ApproveVacationError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}