namespace Arcadia.Assistant.CSP.Configuration
{
    using System.ComponentModel.DataAnnotations;
    using Arcadia.Assistant.Configuration.Configuration;

    public class AccountingReminderConfiguration
    {
        [Required]
        public string DailyRemindTime { get; set; }

        [Required]
        public PushNotification Reminder { get; set; }
    }
}