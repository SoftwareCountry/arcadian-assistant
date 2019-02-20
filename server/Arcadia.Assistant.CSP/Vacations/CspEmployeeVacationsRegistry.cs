namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly string employeeId;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly Dictionary<string, CalendarEvent> eventsById =
            new Dictionary<string, CalendarEvent>();
        private readonly Dictionary<string, List<Approval>> approvalsByEvent =
            new Dictionary<string, List<Approval>>();

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            IRefreshInformation refreshInformation,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.employeeId = employeeId;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes),
                this.Self,
                Refresh.Instance,
                this.Self);

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApprover>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromApprovers>(this.Self);
        }

        public IStash Stash { get; set; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Refresh _:
                    this.vacationsSyncExecutor.GetVacations(this.employeeId)
                        .PipeTo(
                            this.Self,
                            success: result => new RefreshSuccess(result),
                            failure: err => new RefreshFailed(err)
                        );

                    this.Become(this.OnRefreshingVacationsReceive);

                    break;

                case GetCalendarEvents _:
                    this.Sender.Tell(new GetCalendarEvents.Response(this.employeeId, this.eventsById.Values.ToList()));
                    break;

                case GetCalendarEvent msg when this.eventsById.ContainsKey(msg.EventId):
                    this.Sender.Tell(new GetCalendarEvent.Response.Found(this.eventsById[msg.EventId]));
                    break;

                case GetCalendarEvent _:
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEventApprovals msg when this.approvalsByEvent.ContainsKey(msg.Event.EventId):
                    this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(this.approvalsByEvent[msg.Event.EventId]));
                    break;

                case GetCalendarEventApprovals msg:
                    this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse($"Event with event id {msg.Event.EventId} is not found"));
                    break;

                case UpsertCalendarEvent msg when !this.eventsById.ContainsKey(msg.Event.EventId):
                    try
                    {
                        if (msg.Event.Status != this.GetInitialStatus())
                        {
                            throw new Exception($"Event {msg.Event.EventId}. Initial status must be {this.GetInitialStatus()}");
                        }

                        this.EnsureDatesAreNotIntersected(msg.Event);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                        break;
                    }

                    var calendarEvent = new CalendarEvent(null, msg.Event.Type, msg.Event.Dates, msg.Event.Status, msg.Event.EmployeeId);

                    this.vacationsSyncExecutor.InsertVacation(calendarEvent, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                this.UpdateVacationsCache(result);

                                //Context.System.EventStream.Publish(new CalendarEventCreated(calendarEvent, msg.UpdatedBy, msg.Timestamp));
                                return new UpsertCalendarEvent.Success(result.CalendarEvent);
                            },
                            failure: err => new UpsertCalendarEvent.Error(err.Message));

                    break;

                case UpsertCalendarEvent msg:
                    var oldEvent = this.eventsById[msg.Event.EventId];

                    try
                    {
                        var newEvent = msg.Event;
                        if (oldEvent.Status != newEvent.Status && !this.IsStatusTransitionAllowed(oldEvent.Status, newEvent.Status))
                        {
                            throw new Exception(
                                $"Event {msg.Event.EventId}. Status transition {oldEvent.Status} -> {msg.Event.Status} " +
                                "is not allowed for vacation");
                        }

                        this.EnsureDatesAreNotIntersected(msg.Event);

                        if (oldEvent.Dates != newEvent.Dates)
                        {
                            if (!oldEvent.IsPending)
                            {
                                throw new Exception($"Date change is not allowed in status {oldEvent.Status} for {oldEvent.Type}");
                            }

                            if (this.approvalsByEvent.TryGetValue(oldEvent.EventId, out var approvals) && approvals.Count != 0)
                            {
                                throw new Exception($"Date change is not allowed when there is at least one user approval for {oldEvent.Type}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                        break;
                    }

                    this.vacationsSyncExecutor.UpdateVacation(msg.Event, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                this.UpdateVacationsCache(result);

                                //Context.System.EventStream.Publish(new CalendarEventChanged(
                                //    oldEvent,
                                //    msg.UpdatedBy,
                                //    msg.Timestamp,
                                //    result.CalendarEvent));

                                return new UpsertCalendarEvent.Success(result.CalendarEvent);
                            },
                            failure: err => new UpsertCalendarEvent.Error(err.Message));

                    break;

                case ApproveCalendarEvent msg:
                    try
                    {
                        this.ApproveCalendarEvent(msg);
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(ex.Message));
                    }

                    break;

                case CalendarEventAssignedToApprover msg when this.eventsById.ContainsKey(msg.Event.EventId):
                    this.OnCalendarEventNextApproverReceived(msg.Event.EventId, msg.ApproverId);
                    break;

                case CalendarEventRemovedFromApprovers msg when this.eventsById.ContainsKey(msg.Event.EventId):
                    this.OnCalendarEventNextApproverReceived(msg.Event.EventId, null);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnRefreshingVacationsReceive(object message)
        {
            switch (message)
            {
                case Refresh _:
                    // Ignore because it is already in progress
                    break;

                case RefreshSuccess msg:
                    this.eventsById.Clear();
                    this.approvalsByEvent.Clear();

                    foreach (var vacation in msg.Vacations.Where(v => this.IsCalendarEventActual(v.CalendarEvent)))
                    {
                        this.eventsById[vacation.CalendarEvent.EventId] = vacation.CalendarEvent;
                        this.approvalsByEvent[vacation.CalendarEvent.EventId] = vacation.Approvals.ToList();
                    }

                    this.Become(this.DefaultState());

                    break;

                case RefreshFailed msg:
                    this.logger.Error(msg.Exception, $"Failed to load vacations information for employee {this.employeeId}: {msg.Exception.Message}");
                    this.Become(this.DefaultState());

                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void UpdateVacationsCache(CalendarEventWithApprovals result)
        {
            if (this.IsCalendarEventActual(result.CalendarEvent))
            {
                this.eventsById[result.CalendarEvent.EventId] = result.CalendarEvent;
                this.approvalsByEvent[result.CalendarEvent.EventId] = result.Approvals.ToList();
            }
            else if (this.eventsById.ContainsKey(result.CalendarEvent.EventId))
            {
                this.eventsById.Remove(result.CalendarEvent.EventId);
                this.approvalsByEvent.Remove(result.CalendarEvent.EventId);
            }
        }

        private void ApproveCalendarEvent(ApproveCalendarEvent message)
        {
            var calendarEvent = this.eventsById[message.Event.EventId];
            var approvals = this.approvalsByEvent[message.Event.EventId];

            if (!calendarEvent.IsPending)
            {
                var errorMessage = $"Approval of non-pending event {message.Event} is not allowed";
                this.Sender.Tell(new ApproveCalendarEvent.BadRequestResponse(errorMessage));
                return;
            }

            if (approvals.Any(a => a.ApprovedBy == message.ApproverId))
            {
                this.Sender.Tell(Calendar.Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                return;
            }

            this.vacationsSyncExecutor.UpsertVacationApproval(calendarEvent, message.Timestamp, message.ApproverId)
                .PipeTo(
                    this.Sender,
                    success: result =>
                    {
                        this.approvalsByEvent[calendarEvent.EventId].Add(result);

                        //Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(calendarEvent, approvals.ToList()));
                        return Calendar.Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance;
                    },
                    failure: err => new ApproveCalendarEvent.ErrorResponse(err.Message));
        }

        private void OnCalendarEventNextApproverReceived(string eventId, string nextApproverId)
        {
            var oldEvent = this.eventsById[eventId];
            if (!oldEvent.IsPending)
            {
                return;
            }

            if (nextApproverId == null)
            {
                var approvedStatus = new CalendarEventStatuses().ApprovedForType(oldEvent.Type);
                var newEvent = new CalendarEvent(
                    oldEvent.EventId,
                    oldEvent.Type,
                    oldEvent.Dates,
                    approvedStatus,
                    oldEvent.EmployeeId
                );

                var approvals = this.approvalsByEvent[eventId];

                var lastApproval = approvals
                    .OrderByDescending(a => a.Timestamp)
                    .FirstOrDefault();

                // If there is no approvals, then employee is Director General and event is updated by himself
                var updatedBy = lastApproval?.ApprovedBy ?? newEvent.EmployeeId;
                var timestamp = lastApproval?.Timestamp ?? DateTimeOffset.Now;

                this.vacationsSyncExecutor.UpdateVacation(newEvent, timestamp, updatedBy)
                    .PipeTo(
                        this.Sender,
                        success: result =>
                        {
                            this.UpdateVacationsCache(result);

                            //Context.System.EventStream.Publish(new CalendarEventChanged(
                            //    oldEvent,
                            //    msg.UpdatedBy,
                            //    msg.Timestamp,
                            //    result.CalendarEvent));

                            return new UpsertCalendarEvent.Success(result.CalendarEvent);
                        },
                        failure: err => new UpsertCalendarEvent.Error(err.Message));
            }
        }

        private UntypedReceive DefaultState()
        {
            this.Stash.UnstashAll();
            return this.OnReceive;
        }

        private string GetInitialStatus()
        {
            return VacationStatuses.Requested;
        }

        private bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return VacationStatuses.All.Contains(newCalendarEventStatus)
                && (oldCalendarEventStatus != VacationStatuses.Cancelled)
                && (oldCalendarEventStatus != VacationStatuses.Rejected)
                && (newCalendarEventStatus != this.GetInitialStatus())
                && (newCalendarEventStatus != VacationStatuses.Approved);
        }

        private void EnsureDatesAreNotIntersected(CalendarEvent @event)
        {
            var intersectedEvent = this.eventsById.Values
                .Where(ev => ev.EventId != @event.EventId)
                .FirstOrDefault(ev => ev.Dates.DatesIntersectsWith(@event.Dates));
            if (intersectedEvent != null)
            {
                throw new Exception($"Event {@event.EventId}. Dates intersect with another vacation with id {intersectedEvent.EventId}");
            }
        }

        private bool IsCalendarEventActual(CalendarEvent @event)
        {
            return VacationStatuses.Actual.Contains(@event.Status);
        }

        private class Refresh
        {
            public static readonly Refresh Instance = new Refresh();
        }

        private class RefreshSuccess
        {
            public RefreshSuccess(IReadOnlyCollection<CalendarEventWithApprovals> vacations)
            {
                this.Vacations = vacations;
            }

            public IReadOnlyCollection<CalendarEventWithApprovals> Vacations { get; }
        }

        private class RefreshFailed
        {
            public RefreshFailed(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}