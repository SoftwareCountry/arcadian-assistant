namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Calendar.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Models.Calendar;
    using Security;

    public class EditCommonCalendarEventsStatuses : IAuthorizationRequirement
    {
        public CalendarEvent ExistingEvent { get; }
        public CalendarEventsModel UpdatedEvent { get; }

        public EditCommonCalendarEventsStatuses(CalendarEvent existingEvent, CalendarEventsModel updatedEvent)
        {
            ExistingEvent = existingEvent;
            UpdatedEvent = updatedEvent;
        }
    }
}
