using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates
{
    using Assistant.NotificationTemplates;

    using Configuration;

    public class NotificationDataPresenter
    {
        public string ShortBody { get; }

        public string LongBody { get; }

        public string Title { get; }

        public string Subject { get; }

        public NotificationDataPresenter(
            INotificationConfiguration configuration, 
            IDictionary<string, string> context
        )
        {
            this.Subject = configuration.Subject;
            this.Title = configuration.Title;
            this.ShortBody = new TemplateExpressionParser().Parse(configuration.ShortBodyTemplate, context) ?? string.Empty;
            this.LongBody = new TemplateExpressionParser().Parse(configuration.LongBodyTemplate, context) ?? string.Empty;
        }
    }
}
