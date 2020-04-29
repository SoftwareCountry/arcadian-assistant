using System;
using System.Collections.Generic;
using System.Text;

namespace Arcadia.Assistant.NotificationTemplates
{
    using Interfaces.Base;

    public class BodyBuilder
    {
        private string ShortBody { get; }

        private string LongBody { get; }

        public BodyBuilder(INotificationConfiguration configuration, IDictionary<string, string> context)
        {
            this.ShortBody = new TemplateExpressionParser().Parse(configuration.ShortBodyTemplate, context)??string.Empty;
            this.LongBody = new TemplateExpressionParser().Parse(configuration.LongBodyTemplate, context)??string.Empty;
        }
    }
}
