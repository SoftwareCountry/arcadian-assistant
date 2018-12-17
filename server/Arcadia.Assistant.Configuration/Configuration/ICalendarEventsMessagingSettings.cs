namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMessagingSettings
    {
        EmailWithFixedRecipientNotification SickLeaveApprovedEmail { get; }

        EmailNotification EventAssignedToApproverEmail { get; }

        EmailNotification EventStatusChangedEmail { get; }

        EmailNotification EventUserGrantedApprovalEmail { get; }
    }
}