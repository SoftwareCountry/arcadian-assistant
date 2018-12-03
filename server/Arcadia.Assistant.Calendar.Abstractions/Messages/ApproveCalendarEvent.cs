namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class ApproveCalendarEvent
    {
        public ApproveCalendarEvent(CalendarEvent @event, string approverId)
        {
            this.Event = @event;
            this.ApproverId = approverId;
        }

        public CalendarEvent Event { get; }

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