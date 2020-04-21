namespace Arcadia.Assistant.Logging
{
    using System;
    using System.Fabric.Description;

    public class LoggerSettings
    {
        public const string ApplicationInsightsConfigurationKey = "ApplicationInsightsKey";
        public const string SerilogConsoleConfigurationKey = "LogToConsole";
        public const string SerilogFileNameConfigurationKey = "LogFileName";

        public LoggerSettings()
        {
        }

        public LoggerSettings(ConfigurationSection configurationSection)
        {
            this.ApplicationInsightsKey = configurationSection.Parameters[ApplicationInsightsConfigurationKey].Value;
            this.FileLoggingFileName = configurationSection.Parameters[SerilogFileNameConfigurationKey].Value;
            this.ConsoleLoggingOption = Boolean.TryParse(configurationSection.Parameters[SerilogConsoleConfigurationKey].Value, out var value) && value;
        }

        public LoggerSettings(string appKey)
        {
            this.ApplicationInsightsKey = appKey;
        }

        public string ApplicationInsightsKey { get; set; } = string.Empty;

        public bool ConsoleLoggingOption { get; set; }

        public string FileLoggingFileName { get; set; } = string.Empty;

        public static LoggerSettings FromInsightsKey(string applicationInsightsKey)
        {
            return new LoggerSettings(applicationInsightsKey);
        }
    }
}