namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System.Collections.Generic;

    public sealed class GetCalendarEvents
    {
        public static readonly GetCalendarEvents Instance = new GetCalendarEvents();

        public class Response
        {
            public string EmployeeId { get; }

            public IReadOnlyCollection<CalendarEvent> Events { get; }

            public Response(string employeeId, IReadOnlyCollection<CalendarEvent> events)
            {
                this.EmployeeId = employeeId;
                this.Events = events;
            }
        }
    }
}