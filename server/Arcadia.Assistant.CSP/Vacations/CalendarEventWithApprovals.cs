namespace Arcadia.Assistant.CSP.Vacations
{
    using System.Collections.Generic;
    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventWithApprovals
    {
        public CalendarEventWithApprovals(CalendarEvent calendarEvent, IEnumerable<Approval> approvals)
        {
            this.CalendarEvent = calendarEvent;
            this.Approvals = approvals;
        }

        public CalendarEvent CalendarEvent { get; }

        public IEnumerable<Approval> Approvals { get; }
    }
}