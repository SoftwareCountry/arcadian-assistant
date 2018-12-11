namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System.Collections.Generic;

    public class GetCalendarEventApprovals
    {
        public GetCalendarEventApprovals(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }

        public abstract class Response
        {
        }

        public class SuccessResponse : Response
        {
            public SuccessResponse(IEnumerable<Approval> approvals)
            {
                this.Approvals = approvals;
            }

            public IEnumerable<Approval> Approvals { get; }
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