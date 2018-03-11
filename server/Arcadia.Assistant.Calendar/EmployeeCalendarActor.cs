namespace Arcadia.Assistant.Calendar
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    public class EmployeeCalendarActor : UntypedPersistentActor, ILogReceive
    {
        private readonly string employeeId;

        private Dictionary<string, CalendarEvent> eventsById = new Dictionary<string, CalendarEvent>();

        public override string PersistenceId { get; }

        public EmployeeCalendarActor(string employeeId)
        {
            this.employeeId = employeeId;
            this.PersistenceId = $"employee-calendar-{this.employeeId}";
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetCalendarEvents _:
                    this.Sender.Tell(new GetCalendarEvents.Response(this.eventsById.Values.ToList()));
                    break;

                case GetCalendarEvent request when !this.eventsById.ContainsKey(request.EventId):
                    this.Sender.Tell(new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEvent request:
                    this.Sender.Tell(new GetCalendarEvent.Response.Found(this.eventsById[request.EventId]));
                    break;

                case UpsertCalendarEvent cmd:
                    //TODO: split insert and update
                    this.eventsById[cmd.Event.EventId] = cmd.Event;
                    this.Sender.Tell(new UpsertCalendarEvent.Response(cmd.Event));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {

        }

        public static Props CreateProps(string employeeId)
        {
            return Props.Create(() => new EmployeeCalendarActor(employeeId));
        }
    }
}