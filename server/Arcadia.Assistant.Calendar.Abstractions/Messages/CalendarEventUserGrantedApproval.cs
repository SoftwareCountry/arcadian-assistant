namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    using System.Collections.Generic;

    public class CalendarEventUserGrantedApproval
    {
        public CalendarEventUserGrantedApproval(CalendarEvent @event, IEnumerable<string> approvals)
        {
            this.Event = @event;
            this.Approvals = approvals;
        }

        public CalendarEvent Event { get; }

        public IEnumerable<string> Approvals { get; }
    }
}