namespace Arcadia.Assistant.Logging
{
    using System.Fabric.Description;

    public class LoggerSettings
    {
        public const string ApplicationInsightsConfigurationKey = "ApplicationInsightsKey";

        public LoggerSettings()
        {
        }

        public LoggerSettings(ConfigurationSection configurationSection)
        {
            this.ApplicationInsightsKey = configurationSection.Parameters[ApplicationInsightsConfigurationKey].Value;
        }

        public LoggerSettings(string appKey)
        {
            this.ApplicationInsightsKey = appKey;
        }

        public string ApplicationInsightsKey { get; set; } = string.Empty;

        public static LoggerSettings FromInsightsKey(string applicationInsightsKey)
        {
            return new LoggerSettings(applicationInsightsKey);
        }
    }
}