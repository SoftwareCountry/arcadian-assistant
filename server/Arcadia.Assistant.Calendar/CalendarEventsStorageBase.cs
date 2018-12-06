namespace Arcadia.Assistant.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public abstract class CalendarEventsStorageBase : UntypedPersistentActor, ILogReceive
    {
        protected delegate void OnSuccessfulUpsertCallback(CalendarEvent changedEvent);

        protected string EmployeeId { get; }

        protected readonly Dictionary<string, CalendarEvent> EventsById = new Dictionary<string, CalendarEvent>();
        protected readonly Dictionary<string, List<string>> ApprovalsByEvent = new Dictionary<string, List<string>>();

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef calendarEventsApprovalsChecker;

        protected CalendarEventsStorageBase(string employeeId, IActorRef calendarEventsApprovalsChecker)
        {
            this.EmployeeId = employeeId;
            this.calendarEventsApprovalsChecker = calendarEventsApprovalsChecker;
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetCalendarEvents _:
                    this.Sender.Tell(new GetCalendarEvents.Response(this.EmployeeId, this.EventsById.Values.ToList()));
                    break;

                case GetCalendarEvent request when !this.EventsById.ContainsKey(request.EventId):
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEvent request:
                    this.Sender.Tell(new GetCalendarEvent.Response.Found(this.EventsById[request.EventId]));
                    break;

                case UpsertCalendarEvent cmd when cmd.Event.EventId == null:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(new ArgumentNullException(nameof(cmd.Event.EventId)).Message));
                    break;

                //insert
                case UpsertCalendarEvent cmd when !this.EventsById.ContainsKey(cmd.Event.EventId):
                    try
                    {
                        if (cmd.Event.Status != this.GetInitialStatus())
                        {
                            throw new Exception($"Event {cmd.Event.EventId}. Initial status must be {this.GetInitialStatus()}");
                        }

                        this.InsertCalendarEvent(cmd.Event, ev =>
                        {
                            this.OnSuccessfulUpsert(ev);
                            Context.System.EventStream.Publish(new CalendarEventCreated(ev));
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                    }
                    break;

                //update
                case UpsertCalendarEvent cmd:
                    try
                    {
                        var oldEvent = this.EventsById[cmd.Event.EventId];
                        if ((oldEvent.Status != cmd.Event.Status) && !this.IsStatusTransitionAllowed(oldEvent.Status, cmd.Event.Status))
                        {
                            throw new Exception($"Event {cmd.Event.EventId}. Status transition {oldEvent.Status} -> {cmd.Event.Status} is not allowed for {oldEvent.Type}");
                        }
                        this.UpdateCalendarEvent(oldEvent, cmd.Event, ev =>
                        {
                            this.OnSuccessfulUpsert(ev);
                            Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, ev));
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new UpsertCalendarEvent.Error(ex.Message));
                    }
                    break;

                case GetCalendarEventApprovals msg:
                    if (!this.ApprovalsByEvent.TryGetValue(msg.Event.EventId, out var approvals))
                    {
                        this.Sender.Tell(new GetCalendarEventApprovals.ErrorResponse($"Event with event id {msg.Event.EventId} is not found"));
                    }

                    this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(approvals));
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

                case ProcessCalendarEventApprovalsMessage msg:
                    var calendarEvent = this.EventsById[msg.EventId];
                    var existingApprovals = this.ApprovalsByEvent[msg.EventId];

                    this.calendarEventsApprovalsChecker
                        .Ask<GetNextCalendarEventApprover.Response>(
                            new GetNextCalendarEventApprover(this.EmployeeId, existingApprovals, calendarEvent.Type))
                        .ContinueWith<ProcessCalendarEventApprovalsMessage.Response>(task =>
                        {
                            if (task.Result is GetNextCalendarEventApprover.ErrorResponse err)
                            {
                                return new ProcessCalendarEventApprovalsMessage.ErrorResponse(msg.EventId, err.Message);
                            }

                            var resp = (GetNextCalendarEventApprover.SuccessResponse)task.Result;
                            return new ProcessCalendarEventApprovalsMessage.SuccessResponse(msg.EventId, resp.NextApproverEmployeeId);
                        })
                        .PipeTo(this.Self);
                    break;

                case ProcessCalendarEventApprovalsMessage.SuccessResponse msg:
                    this.ProcessCalendarEventApprovals(msg);
                    break;

                case ProcessCalendarEventApprovalsMessage.ErrorResponse msg:
                    this.logger.Warning($"Failed to get next approver for the event {msg.EventId}");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract void InsertCalendarEvent(CalendarEvent calendarEvent, OnSuccessfulUpsertCallback onUpsert);

        protected abstract void UpdateCalendarEvent(CalendarEvent oldEvent, CalendarEvent newEvent, OnSuccessfulUpsertCallback onUpsert);

        protected abstract string GetInitialStatus();

        protected abstract bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus);

        protected abstract void OnSuccessfulApprove(UserGrantedCalendarEventApproval message);

        private void ApproveCalendarEvent(ApproveCalendarEvent message)
        {
            var calendarEvent = this.EventsById[message.Event.EventId];
            var approvals = this.ApprovalsByEvent[message.Event.EventId];

            if (!calendarEvent.IsPending)
            {
                var errorMessage = $"Approval of non-pending event {message.Event} is not allowed";
                this.Sender.Tell(new ApproveCalendarEvent.BadRequestResponse(errorMessage));
                return;
            }

            if (approvals.Contains(message.ApproverId))
            {
                this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                return;
            }

            var @event = new UserGrantedCalendarEventApproval
            {
                EventId = message.Event.EventId,
                TimeStamp = DateTimeOffset.Now,
                ApproverId = message.ApproverId
            };

            this.Persist(@event, ev =>
            {
                this.Self.Tell(new ProcessCalendarEventApprovalsMessage(message.Event.EventId));
                this.Sender.Tell(Abstractions.Messages.ApproveCalendarEvent.SuccessResponse.Instance);
                Context.System.EventStream.Publish(new CalendarEventUserGrantedApproval(calendarEvent, approvals.ToList()));
                this.OnSuccessfulApprove(ev);
            });
        }

        private void ProcessCalendarEventApprovals(ProcessCalendarEventApprovalsMessage.SuccessResponse successResponse)
        {
            var oldEvent = this.EventsById[successResponse.EventId];
            var approvedStatus = new CalendarEventStatuses().ApprovedForType(oldEvent.Type);

            if (successResponse.NextApproverId == null)
            {
                var newEvent = new CalendarEvent(
                    oldEvent.EventId,
                    oldEvent.Type,
                    oldEvent.Dates,
                    approvedStatus,
                    oldEvent.EmployeeId
                );

                this.UpdateCalendarEvent(oldEvent, newEvent, ev =>
                {
                    Context.System.EventStream.Publish(new CalendarEventChanged(oldEvent, ev));
                });
            }

            Context.System.EventStream.Publish(
                new CalendarEventAssignedToApproverEventBusMessage(oldEvent, successResponse.NextApproverId));
        }

        private void OnSuccessfulUpsert(CalendarEvent calendarEvent)
        {
            this.Self.Tell(new ProcessCalendarEventApprovalsMessage(calendarEvent.EventId));
            this.Sender.Tell(new UpsertCalendarEvent.Success(calendarEvent));
        }

        protected class ProcessCalendarEventApprovalsMessage
        {
            public ProcessCalendarEventApprovalsMessage(string eventId)
            {
                this.EventId = eventId;
            }

            public string EventId { get; }

            public abstract class Response
            {
            }

            public class SuccessResponse : Response
            {
                public SuccessResponse(string eventId, string nextApproverId)
                {
                    this.EventId = eventId;
                    this.NextApproverId = nextApproverId;
                }

                public string EventId { get; }

                public string NextApproverId { get; }
            }

            public class ErrorResponse : Response
            {
                public ErrorResponse(string eventId, string message)
                {
                    this.EventId = eventId;
                    this.Message = message;
                }

                public string EventId { get; }

                public string Message { get; }
            }
        }
    }
}