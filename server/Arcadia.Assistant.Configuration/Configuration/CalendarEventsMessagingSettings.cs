namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsMessagingSettings : ICalendarEventsMessagingSettings
    {
        [Required]
        public EmailWithFixedRecipientNotification SickLeaveApprovedEmail { get; set; }

        [Required]
        public EmailNotification EventAssignedToApproverEmail { get; set; }

        [Required]
        public EmailNotification EventStatusChangedEmail { get; set; }

        [Required]
        public EmailNotification EventUserGrantedApprovalEmail { get; set; }
    }
}