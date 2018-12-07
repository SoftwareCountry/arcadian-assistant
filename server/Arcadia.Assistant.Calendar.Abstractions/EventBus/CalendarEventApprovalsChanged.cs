namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    using System;
    using System.Collections.Generic;

    public class CalendarEventApprovalsChanged
    {
        public CalendarEventApprovalsChanged(
            CalendarEvent @event,
            string lastApproverId,
            DateTimeOffset lastApprovalDate,
            IEnumerable<string> approvals)
        {
            this.Event = @event;
            this.LastApproverId = lastApproverId;
            this.LastApprovalDate = lastApprovalDate;
            this.Approvals = approvals;
        }

        public CalendarEvent Event { get; }

        public string LastApproverId { get; }

        public IEnumerable<string> Approvals { get; }

        public DateTimeOffset LastApprovalDate { get; }
    }
}