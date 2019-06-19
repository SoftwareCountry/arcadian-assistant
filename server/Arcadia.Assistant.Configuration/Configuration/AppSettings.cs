namespace Arcadia.Assistant.Configuration.Configuration
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AppSettings
    {
        [Required]
        public MessagingSettings Messaging { get; set; }

        [Required]
        public OrganizationSettings Organization { get; set; }

        [Required]
        public int TimeoutSeconds { get; set; }

        [Required]
        public int VacationsPendingActionsRefreshDays { get; set; }

        public ApplicationInsightsSettings ApplicationInsights { get; set; }

        public TimeSpan Timeout => TimeSpan.FromSeconds(this.TimeoutSeconds);

        public TimeSpan VacationsPendingActionsRefresh => TimeSpan.FromDays(this.VacationsPendingActionsRefreshDays);
    }
}