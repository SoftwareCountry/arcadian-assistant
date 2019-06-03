namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMailSettings
    {
        EmailWithFixedRecipientNotification SickLeaveCreated { get; }

        EmailWithFixedRecipientNotification SickLeaveProlonged { get; }

        EmailWithFixedRecipientNotification SickLeaveCancelled { get; }

        EmailNotification EventAssignedToApprover { get; }

        EmailNotification EventStatusChanged { get; }

        EmailNotification EventUserGrantedApproval { get; }
    }
}