namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class ApproveVacation
    {
        public ApproveVacation(string eventId, string approverId)
        {
            this.EventId = eventId;
            this.ApproverId = approverId;
        }

        public string EventId { get; }

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
    }
}