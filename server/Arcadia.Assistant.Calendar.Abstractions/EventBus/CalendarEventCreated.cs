namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    using System;

    public class CalendarEventCreated
    {
        public CalendarEventCreated(CalendarEvent @event, string creatorId, DateTimeOffset createdAt)
        {
            this.Event = @event;
            this.CreatorId = creatorId;
            this.CreatedAt = createdAt;
        }

        public CalendarEvent Event { get; }

        public string CreatorId { get; }

        public DateTimeOffset CreatedAt { get; }
    }
}