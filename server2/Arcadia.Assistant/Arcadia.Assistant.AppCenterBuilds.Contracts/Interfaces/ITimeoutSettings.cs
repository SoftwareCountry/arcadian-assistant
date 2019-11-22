namespace Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces
{
    using System;

    public interface ITimeoutSettings
    {
        TimeSpan Timeout { get; }
    }
}