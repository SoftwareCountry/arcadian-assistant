namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsMessagingSettings
    {
        EmailWithFixedAddressesSettings SickLeaveApproved { get; set; }

        EmailSettings EventAssignedToApprover { get; set; }

        EmailSettings EventStatusChanged { get; set; }

        EmailSettings EventChangedByOwner { get; set; }
    }
}