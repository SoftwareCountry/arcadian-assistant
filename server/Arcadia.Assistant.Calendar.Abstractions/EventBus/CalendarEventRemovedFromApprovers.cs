namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventRemovedFromApprovers
    {
        public CalendarEventRemovedFromApprovers(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }
    }
}