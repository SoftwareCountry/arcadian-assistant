namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    using System;

    public class CalendarEventChanged
    {
        public CalendarEventChanged(
            CalendarEvent oldEvent,
            string updatedBy,
            DateTimeOffset timestamp,
            CalendarEvent newEvent)
        {
            this.OldEvent = oldEvent;
            this.NewEvent = newEvent;
            this.UpdatedBy = updatedBy;
            this.Timestamp = timestamp;
        }

        public CalendarEvent OldEvent { get; }

        public CalendarEvent NewEvent { get; }

        public string UpdatedBy { get; }

        public DateTimeOffset Timestamp { get; }
    }
}