namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System;
    using System.Collections.Generic;

    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventWithAdditionalData
    {
        public CalendarEventWithAdditionalData(
            CalendarEvent calendarEvent,
            IEnumerable<Approval> approvals,
            SickLeaveCancellation cancelled,
            SickLeaveRejection rejected,
            SickLeaveCompletion completed)
        {
            this.CalendarEvent = calendarEvent;
            this.Approvals = approvals;
            this.Cancelled = cancelled;
            this.Rejected = rejected;
            this.Completed = completed;
        }

        public CalendarEvent CalendarEvent { get; }

        public IEnumerable<Approval> Approvals { get; }

        public SickLeaveCancellation Cancelled { get; }

        public SickLeaveRejection Rejected { get; }

        public SickLeaveCompletion Completed { get; }

        public class SickLeaveCancellation
        {
            public SickLeaveCancellation(string cancelledBy, DateTimeOffset timestamp)
            {
                this.CancelledBy = cancelledBy;
                this.Timestamp = timestamp;
            }

            public string CancelledBy { get; }

            public DateTimeOffset Timestamp { get; set; }
        }

        public class SickLeaveRejection
        {
            public SickLeaveRejection(string rejectedBy, DateTimeOffset timestamp)
            {
                this.RejectedBy = rejectedBy;
                this.Timestamp = timestamp;
            }

            public string RejectedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        public class SickLeaveCompletion
        {
            public SickLeaveCompletion(string completedBy, DateTimeOffset timestamp)
            {
                this.CompletedBy = completedBy;
                this.Timestamp = timestamp;
            }

            public string CompletedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }
    }
}