namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Models.Calendar;
    using Security;

    public class EditCalendarEvents : IAuthorizationRequirement
    {
        public CalendarEvent ExistingEvent { get; }
        public CalendarEventsModel UpdatedEvent { get; }

        public EditCalendarEvents(CalendarEvent existingEvent, CalendarEventsModel updatedEvent)
        {
            ExistingEvent = existingEvent;
            UpdatedEvent = updatedEvent;
        }
    }
}
