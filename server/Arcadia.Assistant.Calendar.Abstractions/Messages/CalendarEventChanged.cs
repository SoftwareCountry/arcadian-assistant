using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.Calendar.Abstractions.Messages
{
    public class CalendarEventChanged
    {
        public CalendarEventChanged(CalendarEvent oldEvent, CalendarEvent newEvent)
        {
            this.OldEvent = oldEvent;
            this.NewEvent = newEvent;
        }

        public CalendarEvent OldEvent { get; }

        public CalendarEvent NewEvent { get; }
    }
}