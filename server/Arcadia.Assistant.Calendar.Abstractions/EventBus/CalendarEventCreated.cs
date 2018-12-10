namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    using System;

    public class CalendarEventCreated
    {
        public CalendarEventCreated(CalendarEvent @event, string createdBy, DateTimeOffset timestamp)
        {
            this.Event = @event;
            this.CreatedBy = createdBy;
            this.Timestamp = timestamp;
        }

        public CalendarEvent Event { get; }

        public string CreatedBy { get; }

        public DateTimeOffset Timestamp { get; }
    }
}