namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System.Collections.Generic;

    public class GetEmployeePendingActions
    {
        public GetEmployeePendingActions(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }

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