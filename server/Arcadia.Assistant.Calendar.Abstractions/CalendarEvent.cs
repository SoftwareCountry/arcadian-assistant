namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System;
    using System.Linq;

    public class CalendarEvent
    {
        public string EventId { get; }

        public DatesPeriod Dates { get; }

        public string Status { get; }

        public string Type { get; }

        public bool IsPending { get; }

        public string EmployeeId { get; }

        public DateTimeOffset CreateDate { get; }

        public DateTimeOffset UpdateDate { get; }

        public string UpdateEmployeeId { get; }

        public CalendarEvent(
            string eventId,
            string type,
            DatesPeriod dates,
            string status,
            string employeeId,
            DateTimeOffset createDate,
            DateTimeOffset updateDate,
            string updateEmployeeId)
        {
            this.EventId = eventId;
            this.Dates = dates;
            this.Status = status;
            this.Type = type;
            this.EmployeeId = employeeId;
            this.CreateDate = createDate;
            this.UpdateDate = updateDate;
            this.UpdateEmployeeId = updateEmployeeId;
            this.IsPending = new CalendarEventStatuses().PendingForType(type).Contains(status);
        }
    }
}