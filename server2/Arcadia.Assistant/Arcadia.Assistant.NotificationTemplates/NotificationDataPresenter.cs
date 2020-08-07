namespace Arcadia.Assistant.NotificationTemplates
{
    using System.Collections.Generic;

    using Configuration;

    public class NotificationDataPresenter
    {
        public NotificationDataPresenter(
            INotificationConfiguration configuration,
            IDictionary<string, string> context
        )
        {
            this.Subject = configuration.Subject;
            this.Title = configuration.Title;
            this.ShortBody = new TemplateExpressionParser().Parse(configuration.ShortBodyTemplate, context) ??
                string.Empty;
            this.LongBody = new TemplateExpressionParser().Parse(configuration.LongBodyTemplate, context) ??
                string.Empty;
        }

        public string ShortBody { get; }

        public string LongBody { get; }

        public string Title { get; }

        public string Subject { get; }
    }
}