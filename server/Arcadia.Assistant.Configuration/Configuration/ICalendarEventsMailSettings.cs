namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMailSettings
    {
        EmailWithFixedRecipientNotification SickLeaveCreatedAccounting { get; }

        EmailWithFixedRecipientNotification SickLeaveProlongedAccounting { get; }

        EmailWithFixedRecipientNotification SickLeaveCancelledAccounting { get; }

        EmailNotification SickLeaveCreatedManager { get; }

        EmailNotification SickLeaveProlongedManager { get; }

        EmailNotification EventAssignedToApprover { get; }

        EmailNotification EventStatusChanged { get; }

        EmailNotification EventUserGrantedApproval { get; }
    }
}