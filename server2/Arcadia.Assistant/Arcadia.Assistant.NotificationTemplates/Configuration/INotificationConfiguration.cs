using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates.Configuration
{
    public interface INotificationConfiguration
    {
        string Subject { get; }
        string Title { get; }
        string ShortBodyTemplate { get; }
        string LongBodyTemplate { get; }
    }
}
