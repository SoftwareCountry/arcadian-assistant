namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventRecoverComplete
    {
        public CalendarEventRecoverComplete(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }
    }
}