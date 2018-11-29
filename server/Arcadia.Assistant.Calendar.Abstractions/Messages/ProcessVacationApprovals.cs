namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class ProcessVacationApprovals
    {
        public ProcessVacationApprovals(string eventId)
        {
            this.EventId = eventId;
        }

        public string EventId { get; }

        public class Response
        {
            public Response(string eventId, string nextApproverId)
            {
                this.EventId = eventId;
                this.NextApproverId = nextApproverId;
            }

            public string EventId { get; }

            public string NextApproverId { get; }
        }
    }
}