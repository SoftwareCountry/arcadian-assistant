namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly string employeeId;

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.employeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventRemovedFromApprovers>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetCalendarEvents _:
                    this.GetVacations()
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                var vacations = result.Select(x => x.CalendarEvent).ToList();
                                return new GetCalendarEvents.Response(this.employeeId, vacations);
                            });
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
                            failure: err => new GetCalendarEventApprovals.ErrorResponse(err.Message));
                    break;

                case UpsertCalendarEvent msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result =>
                            {
                                if (result == null)
                                {
                                    return new InsertCalendarEvent(msg.Event, msg.UpdatedBy, msg.Timestamp);
                                }

                                return new UpdateCalendarEvent(
                                    msg.Event,
                                    result,
                                    msg.UpdatedBy,
                                    msg.Timestamp);
                            },
                            err => new UpsertCalendarEvent.Error(err.Message));
                    break;

                case UpsertCalendarEvent.Error msg:
                    this.Sender.Tell(msg);
                    break;

                case InsertCalendarEvent msg:
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
                        this.Sender.Tell(new InsertCalendarEvent.Error(ex));
                        break;
                    }

                    this.vacationsSyncExecutor.InsertVacation(msg.Event, msg.Timestamp, msg.CreatedBy)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new InsertCalendarEvent.Success(result.CalendarEvent, msg.CreatedBy, msg.Timestamp),
                            err => new InsertCalendarEvent.Error(err));

                    break;

                case InsertCalendarEvent.Success msg:
                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Event, msg.CreatedBy, msg.Timestamp));
                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.Event));
                    break;

                case InsertCalendarEvent.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.Message));
                    break;

                case UpdateCalendarEvent msg:
                    var oldEvent = msg.OldEvent.CalendarEvent;

                    try
                    {
                        var newEvent = msg.NewEvent;
                        if (oldEvent.Status != newEvent.Status && !this.IsStatusTransitionAllowed(oldEvent.Status, newEvent.Status))
                        {
                            throw new Exception(
                                $"Event {msg.NewEvent.EventId}. Status transition {oldEvent.Status} -> {msg.NewEvent.Status} " +
                                "is not allowed for vacation");
                        }

                        this.EnsureDatesAreNotIntersected(msg.NewEvent);

                        if (oldEvent.Dates != newEvent.Dates)
                        {
                            if (!oldEvent.IsPending)
                            {
                                throw new Exception($"Date change is not allowed in status {oldEvent.Status} for {oldEvent.Type}");
                            }

                            var approvals = msg.OldEvent.Approvals;
                            if (approvals.Count() != 0)
                            {
                                throw new Exception($"Date change is not allowed when there is at least one user approval for {oldEvent.Type}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpdateCalendarEvent.Error(ex));
                        break;
                    }

                    this.vacationsSyncExecutor.UpdateVacation(msg.NewEvent, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new UpdateCalendarEvent.Success(result.CalendarEvent, oldEvent, msg.UpdatedBy, msg.Timestamp),
                            err => new UpdateCalendarEvent.Error(err));

                    break;

                case UpdateCalendarEvent.Success msg:
                    Context.System.EventStream.Publish(new CalendarEventChanged(
                        msg.OldEvent,
                        msg.UpdatedBy,
                        msg.Timestamp,
                        msg.NewEvent));

                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.NewEvent));

                    break;

                case UpdateCalendarEvent.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.Message));
                    break;

                case ApproveCalendarEvent msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result =>
                            {
                                if (result != null)
                                {
                                    return new ApproveCalendarEventWithAdditionalData(result, msg.ApproverId, msg.Timestamp);
                                }

                                return new ApproveCalendarEvent.ErrorResponse($"Vacation with id {msg.Event.EventId} is not found");
                            },
                            err => new ApproveCalendarEvent.ErrorResponse(err.Message));
                    break;

                case ApproveCalendarEvent.ErrorResponse msg:
                    this.Sender.Tell(msg);
                    break;

                case ApproveCalendarEventWithAdditionalData msg:
                    this.GrantCalendarEventApproval(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new ApproveCalendarEventWithAdditionalData.Success(result),
                            err => new ApproveCalendarEventWithAdditionalData.Error(err));
                    break;

                case ApproveCalendarEventWithAdditionalData.Success msg:
                    if (msg.NewEvent != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(
                            msg.NewEvent.CalendarEvent,
                            msg.NewEvent.Approvals.ToList()));
                    }

                    this.Sender.Tell(Calendar.Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);

                    break;

                case ApproveCalendarEventWithAdditionalData.Error msg:
                    if (msg.Exception is ArgumentException)
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.BadRequestResponse(msg.Exception.Message));
                    }
                    else
                    {
                        this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(msg.Exception.Message));
                    }

                    break;

                case CalendarEventRemovedFromApprovers msg:
                    this.ApproveCalendarEvent(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new CalendarEventApproved(result));
                    break;

                case CalendarEventApproved msg:
                    if (msg.Data != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventChanged(
                            msg.Data.OldEvent,
                            msg.Data.UpdatedBy,
                            msg.Data.Timestamp,
                            msg.Data.NewEvent));
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
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

        private async Task<CalendarEventWithApprovals> GrantCalendarEventApproval(ApproveCalendarEventWithAdditionalData message)
        {
            var calendarEvent = message.Event.CalendarEvent;
            var approvals = message.Event.Approvals;

            if (!calendarEvent.IsPending)
            {
                var errorMessage = $"Approval of non-pending event {message.Event.CalendarEvent.EventId} is not allowed";
                throw new ArgumentException(errorMessage);
                //var approveResponse = new ApproveCalendarEvent.BadRequestResponse(errorMessage);
            }

            if (approvals.Any(a => a.ApprovedBy == message.ApprovedBy))
            {
                return null;
            }

            await this.vacationsSyncExecutor.UpsertVacationApproval(calendarEvent, message.Timestamp, message.ApprovedBy);

            var newEvent = await this.GetVacation(calendarEvent.EventId);
            return newEvent;
        }

        private async Task<CalendarEventApprovedData> ApproveCalendarEvent(string eventId)
        {
            var oldVacation = await this.GetVacation(eventId);

            var oldEvent = oldVacation?.CalendarEvent;
            if (oldEvent?.IsPending != true)
            {
                return null;
            }

            var approvedStatus = new CalendarEventStatuses().ApprovedForType(oldEvent.Type);
            var newEvent = new CalendarEvent(
                oldEvent.EventId,
                oldEvent.Type,
                oldEvent.Dates,
                approvedStatus,
                oldEvent.EmployeeId
            );

            var approvals = oldVacation.Approvals;

            var lastApproval = approvals
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefault();

            // If there is no approvals, then employee is Director General and event is updated by himself
            var updatedBy = lastApproval?.ApprovedBy ?? newEvent.EmployeeId;
            var timestamp = lastApproval?.Timestamp ?? DateTimeOffset.Now;

            var newVacation = await this.vacationsSyncExecutor.UpdateVacation(newEvent, timestamp, updatedBy);

            return new CalendarEventApprovedData(oldEvent, newVacation.CalendarEvent, updatedBy, timestamp);
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
            //var intersectedEvent = this.eventsById.Values
            //    .Where(ev => ev.EventId != @event.EventId)
            //    .FirstOrDefault(ev => ev.Dates.DatesIntersectsWith(@event.Dates));
            //if (intersectedEvent != null)
            //{
            //    throw new Exception($"Event {@event.EventId}. Dates intersect with another vacation with id {intersectedEvent.EventId}");
            //}
        }

        private bool IsCalendarEventActual(CalendarEvent @event)
        {
            return VacationStatuses.Actual.Contains(@event.Status);
        }

        private class InsertCalendarEvent
        {
            public InsertCalendarEvent(CalendarEvent @event, string createdBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.CreatedBy = createdBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent Event { get; }

            public string CreatedBy { get; }

            public DateTimeOffset Timestamp { get; }


            public class Success
            {
                public Success(CalendarEvent @event, string createdBy, DateTimeOffset timestamp)
                {
                    this.Event = @event;
                    this.CreatedBy = createdBy;
                    this.Timestamp = timestamp;
                }

                public CalendarEvent Event { get; }

                public string CreatedBy { get; }

                public DateTimeOffset Timestamp { get; }
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

        private class UpdateCalendarEvent
        {
            public UpdateCalendarEvent(
                CalendarEvent newEvent,
                CalendarEventWithApprovals oldEvent,
                string updatedBy,
                DateTimeOffset timestamp)
            {
                this.NewEvent = newEvent;
                this.OldEvent = oldEvent;
                this.UpdatedBy = updatedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent NewEvent { get; }

            public CalendarEventWithApprovals OldEvent { get; }

            public string UpdatedBy { get; }

            public DateTimeOffset Timestamp { get; }

            public class Success
            {
                public Success(CalendarEvent newEvent, CalendarEvent oldEvent, string updatedBy, DateTimeOffset timestamp)
                {
                    this.NewEvent = newEvent;
                    this.OldEvent = oldEvent;
                    this.UpdatedBy = updatedBy;
                    this.Timestamp = timestamp;
                }

                public CalendarEvent NewEvent { get; }

                public CalendarEvent OldEvent { get; }

                public string UpdatedBy { get; }

                public DateTimeOffset Timestamp { get; }
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

        private class ApproveCalendarEventWithAdditionalData
        {
            public ApproveCalendarEventWithAdditionalData(CalendarEventWithApprovals @event, string approvedBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.ApprovedBy = approvedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEventWithApprovals Event { get; }

            public string ApprovedBy { get; }

            public DateTimeOffset Timestamp { get; }

            public class Success
            {
                public Success(CalendarEventWithApprovals newEvent)
                {
                    this.NewEvent = newEvent;
                }

                public CalendarEventWithApprovals NewEvent { get; }
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

        private class CalendarEventApprovedData
        {
            public CalendarEventApprovedData(
                CalendarEvent oldEvent,
                CalendarEvent newEvent,
                string updatedBy,
                DateTimeOffset timestamp)
            {
                this.OldEvent = oldEvent;
                this.NewEvent = newEvent;
                this.UpdatedBy = updatedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent OldEvent { get; }

            public CalendarEvent NewEvent { get; }

            public string UpdatedBy { get; }

            public DateTimeOffset Timestamp { get; }

        }

        private class CalendarEventApproved
        {
            public CalendarEventApproved(CalendarEventApprovedData data)
            {
                this.Data = data;
            }

            public CalendarEventApprovedData Data { get; }
        }
    }
}