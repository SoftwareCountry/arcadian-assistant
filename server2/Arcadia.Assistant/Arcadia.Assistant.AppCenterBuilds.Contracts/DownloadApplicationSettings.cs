﻿using System.Fabric.Description;
using Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces;

namespace Arcadia.Assistant.AppCenterBuilds.Contracts
{
    public class DownloadApplicationSettings : IDownloadApplicationSettings
    {
        public DownloadApplicationSettings()
        { }

        public DownloadApplicationSettings(ConfigurationSection configurationSection)
        {
            this.ApiToken = configurationSection.Parameters["ApiToken"].Value;
            this.DownloadBuildIntervalMinutes = int.TryParse(configurationSection.Parameters["DownloadBuildIntervalMinutes"].Value, out var res) ? res : 720;
            this.AndroidGetBuildsUrl = configurationSection.Parameters["AndroidGetBuildsUrl"].Value;
            this.AndroidGetBuildDownloadLinkTemplateUrl = configurationSection.Parameters["AndroidGetBuildDownloadLinkTemplateUrl"].Value;
            this.IosGetBuildsUrl = configurationSection.Parameters["IosGetBuildsUrl"].Value;
            this.IosGetBuildDownloadLinkTemplateUrl = configurationSection.Parameters["IosGetBuildDownloadLinkTemplateUrl"].Value;
        }

        public int DownloadBuildIntervalMinutes { get; set; }

        public string ApiToken { get; set; }

        public string AndroidGetBuildsUrl { get; set; }

        public string AndroidGetBuildDownloadLinkTemplateUrl { get; set; }

        public string IosGetBuildsUrl { get; set; }

        public string IosGetBuildDownloadLinkTemplateUrl { get; set; }
    }
}