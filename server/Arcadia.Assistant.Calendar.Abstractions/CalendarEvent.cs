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

        public CalendarEventAdditionalDataEntry[] AdditionalData { get; }

        public CalendarEvent(
            string eventId,
            string type,
            DatesPeriod dates,
            string status,
            string employeeId,
            CalendarEventAdditionalDataEntry[] additionalData = null)
        {
            this.EventId = eventId;
            this.Dates = dates;
            this.Status = status;
            this.Type = type;
            this.EmployeeId = employeeId;
            this.AdditionalData = additionalData ?? new CalendarEventAdditionalDataEntry[0];
            this.IsPending = new CalendarEventStatuses().PendingForType(type).Contains(status);
        }
    }
}