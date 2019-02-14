namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventRecovered
    {
        public CalendarEventRecovered(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }
    }
}