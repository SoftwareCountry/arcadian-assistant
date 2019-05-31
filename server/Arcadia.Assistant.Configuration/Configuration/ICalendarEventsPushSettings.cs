namespace Arcadia.Assistant.Configuration.Configuration
{
    public interface ICalendarEventsPushSettings
    {
        PushNotification SickLeaveCreatedManager { get; }

        PushNotification SickLeaveProlongedManager { get; }

        PushNotification EventAssignedToApprover { get; }

        PushNotification EventStatusChanged { get; }

        PushNotification EventUserGrantedApproval { get; }
    }
}