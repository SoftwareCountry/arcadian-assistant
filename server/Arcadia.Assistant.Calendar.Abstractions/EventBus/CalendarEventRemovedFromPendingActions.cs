namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventRemovedFromPendingActions
    {
        public CalendarEventRemovedFromPendingActions(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }
    }
}