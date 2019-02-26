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
                    this.GetVacations(false)
                        .PipeTo(
                            this.Self,
                            success: result => new Initialize.Success(result),
                            failure: err => new Initialize.Error(err));
                    break;

                case Initialize.Success msg:
                    this.databaseVacationsCache = msg.Events.ToDictionary(x => x.CalendarEvent.EventId);

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
                                this.databaseVacationsCache[msg.Event.EventId] = result;
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
                                this.databaseVacationsCache[msg.OldEvent.EventId] = result;
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
                                    this.databaseVacationsCache[msg.Event.EventId] = result;
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
                        var cacheApprovals = cacheEvent.Approvals.Select(x => x.ApprovedBy);
                        var databaseApprovals = @event.Approvals.Select(x => x.ApprovedBy).ToList();

                        if (cacheApprovals.Intersect(databaseApprovals).Count() != databaseApprovals.Count)
                        {
                            approvalsUpdatedEvents.Add(@event);
                        }
                    }
                }
            }

            var databaseVacationsById = databaseVacations.ToDictionary(x => x.CalendarEvent.EventId);

            var removedEvents = this.databaseVacationsCache
                .Where(x => !databaseVacationsById.ContainsKey(x.Key))
                .Select(x => x.Value)
                .ToList();

            foreach (var @event in createdEvents)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} for employee {this.employeeId} was added to CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventCreated(@event.CalendarEvent, @event.CalendarEvent.EmployeeId, DateTimeOffset.Now));
                this.databaseVacationsCache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in updatedEvents)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} for employee {this.employeeId} was updated in CSP database manually");
                var oldEvent = this.databaseVacationsCache[@event.CalendarEvent.EventId];
                Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent.CalendarEvent, @event.CalendarEvent.EmployeeId, DateTimeOffset.Now, @event.CalendarEvent));

                this.databaseVacationsCache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in approvalsUpdatedEvents)
            {
                this.logger.Debug($"Approvals for vacation with id {@event.CalendarEvent.EventId} for employee {this.employeeId} were updated in CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(@event.CalendarEvent, @event.Approvals));
                this.databaseVacationsCache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in removedEvents)
            {
                this.logger.Debug($"Vacation with id {@event.CalendarEvent.EventId} for employee {this.employeeId} was removed from CSP database manually");
                Context.System.EventStream.Publish(new CalendarEventRemoved(@event.CalendarEvent));
                this.databaseVacationsCache.Remove(@event.CalendarEvent.EventId);
            }
        }

        private async Task<IEnumerable<CalendarEventWithApprovals>> GetVacations(bool onlyActual = true)
        {
            var vacations = await this.vacationsSyncExecutor.GetVacations(this.employeeId);

            if (onlyActual)
            {
                vacations = vacations.Where(v => this.IsCalendarEventActual(v.CalendarEvent)).ToList();
            }

            return vacations;
        }

        private async Task<CalendarEventWithApprovals> GetVacation(string eventId)
        {
            var vacation = await this.vacationsSyncExecutor.GetVacation(this.employeeId, eventId);

            if (vacation == null)
            {
                return null;
            }

            return this.IsCalendarEventActual(vacation.CalendarEvent) ? vacation : null;
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
    }
}