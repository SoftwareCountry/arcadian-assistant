namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMessagingSettings
    {
        EmailWithFixedRecipientNotification SickLeaveApprovedEmail { get; }

        EmailNotification EventAssignedToApproverEmail { get; }

        PushNotification EventAssignedToApproverPush { get; }

        EmailNotification EventStatusChangedEmail { get; }

        PushNotification EventStatusChangedPush { get; }

        EmailNotification EventUserGrantedApprovalEmail { get; }

        PushNotification EventUserGrantedApprovalPush { get; }
    }
}