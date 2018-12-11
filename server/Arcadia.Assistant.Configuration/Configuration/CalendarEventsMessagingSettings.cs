﻿namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class CalendarEventsMessagingSettings : ICalendarEventsMessagingSettings
    {
        [Required]
        public EmailWithFixedAddressesSettings SickLeaveApproved { get; set; }

        [Required]
        public EmailSettings EventAssignedToApprover { get; set; }

        [Required]
        public EmailSettings EventStatusChanged { get; set; }

        [Required]
        public EmailSettings EventChangedByOwner { get; set; }
    }
}