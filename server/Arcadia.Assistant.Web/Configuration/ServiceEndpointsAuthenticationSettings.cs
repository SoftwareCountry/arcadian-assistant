﻿namespace Arcadia.Assistant.Web.Configuration
{
    using System.ComponentModel.DataAnnotations;

    public class ServiceEndpointsAuthenticationSettings : IServiceEndpointsAuthenticationSettings
    {
        [Required]
        public string Realm { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}