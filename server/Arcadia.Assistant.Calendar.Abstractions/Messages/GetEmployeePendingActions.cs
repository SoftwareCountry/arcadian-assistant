namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System.Collections.Generic;

    public class GetEmployeePendingActions
    {
        public static readonly GetEmployeePendingActions Instance = new GetEmployeePendingActions();

        public class Response
        {
            public Response(IEnumerable<CalendarEvent> pendingActions)
            {
                this.PendingActions = pendingActions;
            }

            public IEnumerable<CalendarEvent> PendingActions { get; }
        }
    }
}