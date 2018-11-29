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

        public class Response
        {
            public static readonly Response Instance = new Response();
        }
    }
}