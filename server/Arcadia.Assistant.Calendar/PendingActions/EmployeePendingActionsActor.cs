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
        private readonly string employeeId;
        private readonly Dictionary<string, CalendarEvent> pendingActionEvents = new Dictionary<string, CalendarEvent>();

        public EmployeePendingActionsActor(string employeeId)
        {
            this.employeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventAddedToPendingActions>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemovedFromPendingActions>(this.Self);
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
                    this.pendingActionEvents[msg.Event.EventId] = msg.Event;
                    break;

                case CalendarEventAddedToPendingActions _:
                    // Simply ignore messages with other event ids
                    break;

                case CalendarEventRemovedFromPendingActions msg when this.pendingActionEvents.ContainsKey(msg.Event.EventId):
                    this.pendingActionEvents.Remove(msg.Event.EventId);
                    break;

                case CalendarEventRemovedFromPendingActions _:
                    // Simply ignore messages with other event ids
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}