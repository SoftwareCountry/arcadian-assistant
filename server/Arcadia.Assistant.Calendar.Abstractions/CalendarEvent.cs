namespace Arcadia.Assistant.Calendar.Abstractions
{
    public class CalendarEvent
    {
        public string EventId { get; }

        public DatesPeriod Dates { get; }

        public string Status { get; }

        public CalendarEvent(string eventId, DatesPeriod dates, string status)
        {
            this.EventId = eventId;
            this.Dates = dates;
            this.Status = status;
        }
    }
}