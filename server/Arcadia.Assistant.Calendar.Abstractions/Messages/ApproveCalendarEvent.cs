namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System;

    public class ApproveCalendarEvent
    {
        public ApproveCalendarEvent(CalendarEvent @event, DateTimeOffset timestamp, string approverId)
        {
            this.Event = @event;
            this.Timestamp = timestamp;
            this.ApproverId = approverId;
        }

        public CalendarEvent Event { get; }

        public DateTimeOffset Timestamp { get; }

        public string ApproverId { get; }

        public abstract class Response
        {
        }

        public class SuccessResponse : Response
        {
            public static readonly SuccessResponse Instance = new SuccessResponse();
        }

        public class BadRequestResponse : Response
        {
            public BadRequestResponse(string message)
            {
                this.Message = message;
            }

            public string Message { get; }
        }

        public class ErrorResponse : Response
        {
            public ErrorResponse(string message)
            {
                this.Message = message;
            }

            public string Message { get; }
        }
    }
}