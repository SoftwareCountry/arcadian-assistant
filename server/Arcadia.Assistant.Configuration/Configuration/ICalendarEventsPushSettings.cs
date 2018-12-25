namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsPushSettings
    {
        PushNotification EventAssignedToApprover { get; }

        PushNotification EventStatusChanged { get; }

        PushNotification EventUserGrantedApproval { get; }
    }
}