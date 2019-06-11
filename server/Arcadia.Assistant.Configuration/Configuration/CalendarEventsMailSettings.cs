namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsMailSettings : ICalendarEventsMailSettings
    {
        [Required]
        public EmailWithFixedRecipientNotification SickLeaveCreated { get; set; }

        [Required]
        public EmailWithFixedRecipientNotification SickLeaveProlonged { get; set; }

        [Required]
        public EmailWithFixedRecipientNotification SickLeaveCancelled { get; set; }

        [Required]
        public EmailNotification SickLeaveCreatedManager { get; set; }

        [Required]
        public EmailNotification SickLeaveProlongedManager { get; set; }

        [Required]
        public EmailNotification SickLeaveCancelledManager { get; set; }

        [Required]
        public EmailNotification EventAssignedToApprover { get; set; }

        [Required]
        public EmailNotification EventStatusChanged { get; set; }

        [Required]
        public EmailNotification EventUserGrantedApproval { get; set; }
    }
}