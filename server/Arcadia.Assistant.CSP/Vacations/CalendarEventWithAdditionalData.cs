﻿namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventWithAdditionalData
    {
        public CalendarEventWithAdditionalData(
            CalendarEvent calendarEvent,
            IEnumerable<Approval> approvals,
            VacationProcessing processed,
            VacationCancellation cancelled,
            VacationRejection rejected)
        {
            this.CalendarEvent = calendarEvent;
            this.Approvals = approvals;
            this.Processed = processed;
            this.Cancelled = cancelled;
            this.Rejected = rejected;
        }

        public CalendarEvent CalendarEvent { get; }

        public IEnumerable<Approval> Approvals { get; }

        public VacationProcessing Processed { get; }

        public VacationCancellation Cancelled { get; }

        public VacationRejection Rejected { get; set; }

        public class VacationProcessing
        {
            public VacationProcessing(string processedBy, DateTimeOffset timestamp)
            {
                this.ProcessedBy = processedBy;
                this.Timestamp = timestamp;
            }

            public string ProcessedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        public class VacationCancellation
        {
            public VacationCancellation(string cancelledBy, DateTimeOffset timestamp)
            {
                this.CancelledBy = cancelledBy;
                this.Timestamp = timestamp;
            }

            public string CancelledBy { get; }

            public DateTimeOffset Timestamp { get; set; }
        }

        public class VacationRejection
        {
            public VacationRejection(string rejectedBy, DateTimeOffset timestamp)
            {
                this.RejectedBy = rejectedBy;
                this.Timestamp = timestamp;
            }

            public string RejectedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }
    }
}