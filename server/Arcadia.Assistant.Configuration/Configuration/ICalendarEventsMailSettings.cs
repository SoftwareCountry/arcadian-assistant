namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMailSettings
    {
        EmailWithFixedRecipientNotification SickLeaveApproved { get; }

        EmailNotification EventAssignedToApprover { get; }

        EmailNotification EventStatusChanged { get; }

        EmailNotification EventUserGrantedApproval { get; }
    }
}