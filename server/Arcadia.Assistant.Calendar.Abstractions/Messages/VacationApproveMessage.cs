namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class VacationApproveMessage
    {
        public VacationApproveMessage(string eventId, string approverId)
        {
            this.EventId = eventId;
            this.ApproverId = approverId;
        }

        public string EventId { get; }

        public string ApproverId { get; }

        public class Response
        {
            public static readonly Response Instance = new Response();
        }
    }
}