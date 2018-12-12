namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsMessagingSettings : ICalendarEventsMessagingSettings
    {
        [Required]
        public EmailWithFixedRecipientSettings SickLeaveApproved { get; set; }

        [Required]
        public EmailSettings EventAssignedToApprover { get; set; }

        [Required]
        public EmailSettings EventStatusChanged { get; set; }

        [Required]
        public EmailSettings EventChangedByOwner { get; set; }

        [Required]
        public EmailSettings EventUserGrantedApproval { get; set; }
    }
}