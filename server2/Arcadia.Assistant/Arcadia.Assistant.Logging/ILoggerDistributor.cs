using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.Logging
{
    using Microsoft.Extensions.Logging;

    public interface ILoggerDistributor
    {
        ILogger Logger { get; }
    }
}
