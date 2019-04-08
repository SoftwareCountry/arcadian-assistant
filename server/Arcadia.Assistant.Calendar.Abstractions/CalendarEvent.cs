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

        public IEnumerable<CalendarEventAdditionalDataEntry> AdditionalData { get; }

        public CalendarEvent(
            string eventId,
            string type,
            DatesPeriod dates,
            string status,
            string employeeId,
            IEnumerable<CalendarEventAdditionalDataEntry> additionalData = null)
        {
            this.EventId = eventId;
            this.Dates = dates;
            this.Status = status;
            this.Type = type;
            this.EmployeeId = employeeId;
            this.AdditionalData = additionalData ?? new List<CalendarEventAdditionalDataEntry>();
            this.IsPending = new CalendarEventStatuses().PendingForType(type).Contains(status);
        }
    }
}