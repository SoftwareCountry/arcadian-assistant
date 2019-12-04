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