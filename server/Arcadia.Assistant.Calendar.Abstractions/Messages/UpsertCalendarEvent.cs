namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class UpsertCalendarEvent
    {
        public CalendarEvent Event { get; }

        public UpsertCalendarEvent(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public class Response
        {
            public CalendarEvent Event { get; }

            public Response(CalendarEvent @event)
            {
                this.Event = @event;
            }
        }
    }
}