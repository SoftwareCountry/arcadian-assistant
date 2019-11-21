using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.Logging
{
    public class LoggerSettings
    {
        public LoggerSettings(string appKey)
        {
            this.ApplicationInsightsKey = appKey;
        }

        public string ApplicationInsightsKey { get; set; }
    }
}
