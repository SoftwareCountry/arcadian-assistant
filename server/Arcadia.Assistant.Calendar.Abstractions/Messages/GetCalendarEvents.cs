namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System.Collections.Generic;

    public sealed class GetCalendarEvents
    {
        public static readonly GetCalendarEvents Instance = new GetCalendarEvents();

        public class Response
        {
            public IReadOnlyCollection<CalendarEvent> Events { get; }

            public Response(IReadOnlyCollection<CalendarEvent> events)
            {
                this.Events = events;
            }
        }
    }
}