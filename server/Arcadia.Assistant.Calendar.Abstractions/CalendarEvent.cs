namespace Arcadia.Assistant.Calendar.Abstractions
{
    public class CalendarEvent
    {
        public string EventId { get; }

        public DatesPeriod Dates { get; }

        public CalendarEventStatus Status { get; }

        public CalendarEvent(string eventId, DatesPeriod dates, CalendarEventStatus status)
        {
            this.EventId = eventId;
            this.Dates = dates;
            this.Status = status;
        }
    }
}