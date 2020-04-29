using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates
{
    public class Event
    {
        public string EventId { get; }

        public DateTime StartDate { get; }

        public DateTime EndDate { get; }

        public string Status { get; }

        public string Type { get; }

        public string EmployeeId { get; }

        public EventParameter[] AdditionalData { get; }

        public Event(
            string eventId,
            string type,
            DateTime startDate,
            DateTime endDate,
            string status,
            string employeeId,
            EventParameter[]? additionalData = null)
        {
            this.EventId = eventId;
            this.StartDate = startDate;
            this.EndDate = endDate;
            this.Status = status;
            this.Type = type;
            this.EmployeeId = employeeId;
            this.AdditionalData = additionalData ?? new EventParameter[0];
        }
    }
}
