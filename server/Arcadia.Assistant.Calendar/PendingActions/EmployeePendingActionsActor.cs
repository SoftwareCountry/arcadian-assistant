namespace Arcadia.Assistant.Calendar.PendingActions
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
            this.logger.Debug($"Pending actions actor for employee {this.employeeId}. Message received: {message.GetType().FullName}");

            switch (message)
            {
                case GetEmployeePendingActions _:
                    this.Sender.Tell(new GetEmployeePendingActions.Response(this.pendingActionEvents.Values.ToList()));
                    break;

                case CalendarEventAddedToPendingActions msg when msg.ApproverId == this.employeeId:
                    this.AddPendingAction(msg.Event);
                    break;

                case CalendarEventAddedToPendingActions msg:
                    this.RemovedFromPendingActions(msg.Event);
                    break;

                case CalendarEventRemovedFromPendingActions msg when this.HasPendingAction(msg.Event.EventId):
                    this.RemovedFromPendingActions(msg.Event);
                    break;

                case CalendarEventRemovedFromPendingActions msg:
                    this.logger.Debug($"No actions made for event {msg.Event.EventId}");
                    break;

                case CalendarEventChanged msg when msg.NewEvent.IsPending && this.HasPendingAction(msg.NewEvent.EventId):
                    this.AddPendingAction(msg.NewEvent);
                    break;

                case CalendarEventChanged msg when !msg.NewEvent.IsPending && this.HasPendingAction(msg.NewEvent.EventId):
                    this.RemovedFromPendingActions(msg.NewEvent);
                    break;

                case CalendarEventChanged msg:
                    this.logger.Debug($"No actions made for event {msg.NewEvent.EventId}");
                    break;

                case CalendarEventApprovalsChanged msg when !msg.Event.IsPending && this.HasPendingAction(msg.Event.EventId):
                    this.RemovedFromPendingActions(msg.Event);
                    break;

                case CalendarEventApprovalsChanged msg:
                    this.logger.Debug($"No actions made for event {msg.Event.EventId}");
                    break;

                case CalendarEventRemoved msg when this.HasPendingAction(msg.Event.EventId):
                    this.RemovedFromPendingActions(msg.Event);
                    break;

                case CalendarEventRemoved msg:
                    this.logger.Debug($"No actions made for event {msg.Event.EventId}");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void AddPendingAction(CalendarEvent @event)
        {
            this.logger.Debug($"Pending action added for event {@event.EventId}");

            this.pendingActionEvents[@event.EventId] = @event;
        }

        private void RemovedFromPendingActions(CalendarEvent @event)
        {
            this.logger.Debug($"Pending action removed for event {@event.EventId}");

            if (this.pendingActionEvents.ContainsKey(@event.EventId))
            {
                this.pendingActionEvents.Remove(@event.EventId);
            }
        }

        private bool HasPendingAction(string eventId)
        {
            return this.pendingActionEvents.ContainsKey(eventId);
        }
    }
}