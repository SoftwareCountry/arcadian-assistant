namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    using System;
    using System.Collections.Generic;

    public class CalendarEventApprovalsChanged
    {
        public CalendarEventApprovalsChanged(
            CalendarEvent @event,
            string lastApprovedBy,
            DateTimeOffset lastApprovalTimestamp,
            IEnumerable<string> approvals)
        {
            this.Event = @event;
            this.LastApproval = new Approval(lastApprovedBy, lastApprovalTimestamp);
            this.Approvals = approvals;
        }

        public CalendarEvent Event { get; }

        public Approval LastApproval { get; set; }

        public IEnumerable<string> Approvals { get; }

        public class Approval
        {
            public Approval(string approvedBy, DateTimeOffset timestamp)
            {
                this.ApprovedBy = approvedBy;
                this.Timestamp = timestamp;
            }

            public string ApprovedBy { get; set; }

            public DateTimeOffset Timestamp { get; set; }
        }
    }
}