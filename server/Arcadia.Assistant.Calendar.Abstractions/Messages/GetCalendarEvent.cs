namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class GetCalendarEvent
    {
        public GetCalendarEvent(string eventId)
        {
            this.EventId = eventId;
        }

        public string EventId { get; }

        public abstract class Response
        {
            public class Found : Response
            {
                public CalendarEvent Event { get; }

                public Found(CalendarEvent @event)
                {
                    this.Event = @event;
                }
            }

            public class NotFound : Response
            {
            }
        }
    }
}