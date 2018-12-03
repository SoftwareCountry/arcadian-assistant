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
        private readonly Dictionary<string, CalendarEvent> pendingActionEvents = new Dictionary<string, CalendarEvent>();

        public EmployeeVacationsPendingActionsActor(string employeeId)
        {
            this.employeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventAssignedToApproverEventBusMessage>(this.Self);
        }

        public static Props CreateProps(string employeeId)
        {
            return Props.Create(() => new EmployeeVacationsPendingActionsActor(employeeId));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetEmployeePendingActions _:
                    this.Sender.Tell(new GetEmployeePendingActions.Response(this.pendingActionEvents.Values.ToList()));
                    break;

                case CalendarEventAssignedToApproverEventBusMessage msg when msg.Event.Type != CalendarEventTypes.Vacation:
                    break;

                case CalendarEventAssignedToApproverEventBusMessage msg when msg.ApproverId == this.employeeId:
                    this.pendingActionEvents[msg.Event.EventId] = msg.Event;
                    break;

                case CalendarEventAssignedToApproverEventBusMessage msg
                    when msg.ApproverId != this.employeeId && this.pendingActionEvents.ContainsKey(msg.Event.EventId):

                    this.pendingActionEvents.Remove(msg.Event.EventId);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}