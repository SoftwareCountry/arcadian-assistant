namespace Arcadia.Assistant.AppCenterBuilds.Contracts
{
    using System;

    public interface ITimeoutSettings
    {
        TimeSpan? Timeout { get; }
    }
}