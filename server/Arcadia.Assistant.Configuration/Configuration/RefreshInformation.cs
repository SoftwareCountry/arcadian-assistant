﻿namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class RefreshInformation : IRefreshInformation
    {
        [Required]
        public double IntervalInMinutes { get; set; }
    }
}