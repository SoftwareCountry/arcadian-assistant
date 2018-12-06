namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventCreated
    {
        public CalendarEventCreated(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }
    }
}