﻿namespace Arcadia.Assistant.Calendar.Abstractions.EventBus
{
    using System.Collections.Generic;

    public class CalendarEventApprovalsChanged
    {
        public CalendarEventApprovalsChanged(CalendarEvent @event, IEnumerable<string> approvals)
        {
            this.Event = @event;
            this.Approvals = approvals;
        }

        public CalendarEvent Event { get; }

        public IEnumerable<string> Approvals { get; }
    }
}