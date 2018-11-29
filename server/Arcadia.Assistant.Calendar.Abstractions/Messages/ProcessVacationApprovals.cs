namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class ProcessVacationApprovals
    {
        public ProcessVacationApprovals(string eventId)
        {
            this.EventId = eventId;
        }

        public string EventId { get; }

        public abstract class Response
        {
        }

        public class SuccessResponse : Response
        {
            public SuccessResponse(string eventId, string nextApproverId)
            {
                this.EventId = eventId;
                this.NextApproverId = nextApproverId;
            }

            public string EventId { get; }

            public string NextApproverId { get; }
        }

        public class ErrorResponse : Response
        {
            public ErrorResponse(string eventId, string message)
            {
                this.EventId = eventId;
                this.Message = message;
            }

            public string EventId { get; }

            public string Message { get; }
        }
    }
}