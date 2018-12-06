namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    public class CalendarEventChanged
    {
        public CalendarEventChanged(CalendarEvent oldEvent, CalendarEvent newEvent)
        {
            this.OldEvent = oldEvent;
            this.NewEvent = newEvent;
        }

        public CalendarEvent OldEvent { get; }

        public CalendarEvent NewEvent { get; }
    }
}