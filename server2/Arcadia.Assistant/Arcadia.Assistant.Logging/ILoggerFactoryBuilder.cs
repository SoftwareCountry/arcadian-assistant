namespace Arcadia.Assistant.Logging
{
    using Microsoft.Extensions.Logging;

    public interface ILoggerFactoryBuilder
    {
        ILoggerFactory CreateLoggerFactory(string aiKey);
    }
}