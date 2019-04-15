namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System;

    public class CheckDatesAvailability
    {
        public CheckDatesAvailability(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }

        public abstract class Response
        {
        }

        public class Success : Response
        {
            public Success(bool result)
            {
                this.Result = result;
            }

            public bool Result { get; }
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