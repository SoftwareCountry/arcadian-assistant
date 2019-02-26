namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventRemoved
    {
        public CalendarEventRemoved(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get;  }
    }
}