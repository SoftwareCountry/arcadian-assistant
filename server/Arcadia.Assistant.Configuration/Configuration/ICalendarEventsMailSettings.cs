namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMailSettings
    {
        EmailWithFixedRecipientNotification SickLeaveCreatedAccounting { get; }

        EmailWithFixedRecipientNotification SickLeaveProlongedAccounting { get; }

        EmailNotification EventAssignedToApprover { get; }

        EmailNotification EventStatusChanged { get; }

        EmailNotification EventUserGrantedApproval { get; }
    }
}