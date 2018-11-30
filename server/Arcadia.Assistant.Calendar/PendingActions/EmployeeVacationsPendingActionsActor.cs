namespace Arcadia.Assistant.Calendar.PendingActions
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public class EmployeeVacationsPendingActionsActor : UntypedActor, ILogReceive
    {
        private readonly string employeeId;
        private readonly List<CalendarEvent> pendingActionEvents = new List<CalendarEvent>();

        public EmployeeVacationsPendingActionsActor(string employeeId)
        {
            this.employeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventApproverEventBusMessage>(this.Self);
        }

        public static Props CreateProps(string employeeId)
        {
            return Props.Create(() => new EmployeeVacationsPendingActionsActor(employeeId));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetEmployeePendingActions msg when msg.EmployeeId == this.employeeId:
                    this.Sender.Tell(new GetEmployeePendingActions.Response(this.pendingActionEvents));
                    break;

                case CalendarEventApproverEventBusMessage msg:
                    if (msg.Event.Type != CalendarEventTypes.Vacation)
                    {
                        break;
                    }

                    if (msg.ApproverId == this.employeeId &&
                        this.pendingActionEvents.All(e => e.EventId != msg.Event.EventId))
                    {
                        this.pendingActionEvents.Add(msg.Event);
                    }
                    else if (msg.ApproverId != this.employeeId &&
                        this.pendingActionEvents.Any(e => e.EventId == msg.Event.EventId))
                    {
                        this.pendingActionEvents.RemoveAll(e => e.EventId == msg.Event.EventId);
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}