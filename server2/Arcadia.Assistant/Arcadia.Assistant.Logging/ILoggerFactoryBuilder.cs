using Microsoft.Extensions.Logging;

namespace Arcadia.Assistant.Logging
{
    public interface ILoggerFactoryBuilder
    {
        ILoggerFactory CreateLoggerFactory(string aiKey);
    }
}