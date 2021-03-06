﻿namespace Arcadia.Assistant.Calendar.PendingActions
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public class EmployeePendingActionsActor : UntypedActor, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly string employeeId;
        private readonly Dictionary<string, CalendarEvent> pendingActionEvents = new Dictionary<string, CalendarEvent>();

        public EmployeePendingActionsActor(string employeeId)
        {
            this.employeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventAddedToPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemoved>(this.Self);
        }

        public static Props CreateProps(string employeeId)
        {
            return Props.Create(() => new EmployeePendingActionsActor(employeeId));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetEmployeePendingActions _:
                    this.Sender.Tell(new GetEmployeePendingActions.Response(this.pendingActionEvents.Values.ToList()));
                    break;

                case CalendarEventAddedToPendingActions msg when msg.ApproverId == this.employeeId:
                    this.AddToPendingActions(msg.Event);
                    break;

                case CalendarEventAddedToPendingActions _:
                    break;

                case CalendarEventRemovedFromPendingActions msg when this.HasPendingAction(msg.Event.EventId):
                    this.RemoveFromPendingActions(msg.Event);
                    break;

                case CalendarEventRemovedFromPendingActions _:
                    break;

                case CalendarEventChanged msg when msg.NewEvent.IsPending && this.HasPendingAction(msg.NewEvent.EventId):
                    this.AddToPendingActions(msg.NewEvent);
                    break;

                case CalendarEventChanged msg when !msg.NewEvent.IsPending && this.HasPendingAction(msg.NewEvent.EventId):
                    this.RemoveFromPendingActions(msg.NewEvent);
                    break;

                case CalendarEventChanged _:
                    break;

                case CalendarEventApprovalsChanged msg
                    when msg.Event.IsPending && msg.Approvals.Any(a => a.ApprovedBy == this.employeeId) && this.HasPendingAction(msg.Event.EventId):

                    this.RemoveFromPendingActions(msg.Event);
                    break;

                case CalendarEventApprovalsChanged msg when !msg.Event.IsPending && this.HasPendingAction(msg.Event.EventId):
                    this.RemoveFromPendingActions(msg.Event);
                    break;

                case CalendarEventApprovalsChanged _:
                    break;

                case CalendarEventRemoved msg when this.HasPendingAction(msg.Event.EventId):
                    this.RemoveFromPendingActions(msg.Event);
                    break;

                case CalendarEventRemoved _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void AddToPendingActions(CalendarEvent @event)
        {
            this.logger.Debug($"Employee {this.employeeId}. Pending action added for event {@event.EventId}.");
            this.pendingActionEvents[@event.EventId] = @event;
        }

        private void RemoveFromPendingActions(CalendarEvent @event)
        {
            if (this.pendingActionEvents.ContainsKey(@event.EventId))
            {
                this.logger.Debug($"Employee {this.employeeId}. Pending action removed for event {@event.EventId}");
                this.pendingActionEvents.Remove(@event.EventId);
            }
        }

        private bool HasPendingAction(string eventId)
        {
            return this.pendingActionEvents.ContainsKey(eventId);
        }
    }
}