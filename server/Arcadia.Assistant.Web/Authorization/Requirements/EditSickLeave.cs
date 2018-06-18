using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Models.Calendar;

    public class EditSickLeave : IAuthorizationRequirement
    {
        public CalendarEvent ExistingEvent { get; }
        public CalendarEventsModel UpdatedEvent { get; }

        public EditSickLeave(CalendarEvent existingEvent, CalendarEventsModel updatedEvent)
        {
            ExistingEvent = existingEvent;
            UpdatedEvent = updatedEvent;
        }
    }
}
