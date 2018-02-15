namespace Arcadia.Assistant.Web.Configuration
{
    using System;

    public interface ITimeoutSettings
    {
        TimeSpan Timeout { get; }
    }
}