namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System;

    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventWithAdditionalData
    {
        public CalendarEventWithAdditionalData(
            CalendarEvent calendarEvent,
            SickLeaveCancellation cancelled,
            SickLeaveCompletion completed)
        {
            this.CalendarEvent = calendarEvent;
            this.Cancelled = cancelled;
            this.Completed = completed;
        }

        public CalendarEvent CalendarEvent { get; }

        public SickLeaveCancellation Cancelled { get; }

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