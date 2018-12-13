﻿namespace Arcadia.Assistant.Configuration.Configuration
{
    using System.Collections.Generic;

    public interface IPushSettings
    {
        bool Enabled { get; }

        string ApiToken { get; }

        string ApplicationPushUrl { get; }
    }
}