namespace Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations
{
    using System;
    using System.Collections.Generic;

    public class ApproveVacation
    {
        public ApproveVacation(CalendarEvent @event, string approvedBy, DateTimeOffset timestamp)
        {
            this.Event = @event;
            this.ApprovedBy = approvedBy;
            this.Timestamp = timestamp;
        }

        public CalendarEvent Event { get; }

        public string ApprovedBy { get; }

        public DateTimeOffset Timestamp { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public static readonly Success Instance = new Success(null, null);

            public Success(CalendarEvent @event, IEnumerable<Approval> approvals)
            {
                this.Event = @event;
                this.Approvals = approvals;
            }

            public CalendarEvent Event { get; }

            public IEnumerable<Approval> Approvals { get; }
        }

        public class Error : Response
        {
            public Error(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}