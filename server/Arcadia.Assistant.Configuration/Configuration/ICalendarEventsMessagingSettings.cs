namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMessagingSettings
    {
        EmailWithFixedRecipientSettings SickLeaveApproved { get; }

        EmailSettings EventAssignedToApprover { get; }

        EmailSettings EventStatusChanged { get; }

        EmailSettings EventUserGrantedApproval { get; }
    }
}