namespace Arcadia.Assistant.CSP.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using Arcadia.Assistant.Configuration.Configuration;

    public class SickLeavesAccountingReminderConfiguration
    {
        [Required]
        public string DailyRemindTime { get; set; }

        [Required]
        public PushNotification ReminderPush { get; set; }

        [Required]
        public EmailNotification ReminderEmail { get; set; }
    }
}