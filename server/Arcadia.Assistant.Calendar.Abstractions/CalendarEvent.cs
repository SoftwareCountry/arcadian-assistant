namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System.Collections.Generic;
    using System.Linq;

    public class CalendarEvent
    {
        public string EventId { get; }

        public DatesPeriod Dates { get; }

        public string Status { get; }

        public string Type { get; }

        public bool IsPending { get; }

        public string EmployeeId { get; }

        public IReadOnlyDictionary<string, string> AdditionalData { get; }

        public CalendarEvent(
            string eventId,
            string type,
            DatesPeriod dates,
            string status,
            string employeeId,
            IReadOnlyDictionary<string, string> additionalData = null)
        {
            this.EventId = eventId;
            this.Dates = dates;
            this.Status = status;
            this.Type = type;
            this.EmployeeId = employeeId;
            this.AdditionalData = additionalData;
            this.IsPending = new CalendarEventStatuses().PendingForType(type).Contains(status);
        }
    }
}