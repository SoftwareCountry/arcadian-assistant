namespace Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations
{
    using System;

    public class InsertVacation
    {
        public InsertVacation(CalendarEvent @event, string createdBy, DateTimeOffset timestamp)
        {
            this.Event = @event;
            this.CreatedBy = createdBy;
            this.Timestamp = timestamp;
        }

        public CalendarEvent Event { get; }

        public string CreatedBy { get; }

        public DateTimeOffset Timestamp { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(CalendarEvent @event, string createdBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.CreatedBy = createdBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent Event { get; }

            public string CreatedBy { get; }

            public DateTimeOffset Timestamp { get; }
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