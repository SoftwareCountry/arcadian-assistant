namespace Arcadia.Assistant.Notifications.Email
{
    using System.Collections.Generic;

    public class EmailNotification
    {
        public EmailNotification(string sender, IEnumerable<string> recipients, string subject, string body)
        {
            this.Sender = sender;
            this.Recipients = recipients;
            this.Subject = subject;
            this.Body = body;
        }

        public string Sender { get; }

        public IEnumerable<string> Recipients { get; }

        public string Subject { get; }

        public string Body { get; }
    }
}