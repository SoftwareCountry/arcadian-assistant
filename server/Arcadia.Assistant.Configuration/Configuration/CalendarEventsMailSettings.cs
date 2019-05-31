namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsMailSettings : ICalendarEventsMailSettings
    {
        [Required]
        public EmailWithFixedRecipientNotification SickLeaveCreatedAccounting { get; set; }

        [Required]
        public EmailWithFixedRecipientNotification SickLeaveProlongedAccounting { get; set; }

        public EmailWithFixedRecipientNotification SickLeaveCancelledAccounting { get; set; }

        [Required]
        public EmailNotification SickLeaveCreatedManager { get; set; }

        [Required]
        public EmailNotification SickLeaveProlongedManager { get; set; }

        [Required]
        public EmailNotification EventAssignedToApprover { get; set; }

        [Required]
        public EmailNotification EventStatusChanged { get; set; }

        [Required]
        public EmailNotification EventUserGrantedApproval { get; set; }
    }
}