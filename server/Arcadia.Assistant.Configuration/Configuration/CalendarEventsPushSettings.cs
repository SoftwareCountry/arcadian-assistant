namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsPushSettings : ICalendarEventsPushSettings
    {
        [Required]
        public PushNotification SickLeaveCreatedManager { get; set; }

        [Required]
        public PushNotification SickLeaveProlongedManager { get; set; }

        [Required]
        public PushNotification EventAssignedToApprover { get; set; }

        [Required]
        public PushNotification EventStatusChanged { get; set; }

        [Required]
        public PushNotification EventUserGrantedApproval { get; set; }
    }
}