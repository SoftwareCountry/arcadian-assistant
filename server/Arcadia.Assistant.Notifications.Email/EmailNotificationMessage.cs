namespace Arcadia.Assistant.Notifications.Email
{
    public class EmailNotificationMessage
    {
        public EmailNotificationMessage(string sender, string recipient, string subject, string body)
        {
            this.Sender = sender;
            this.Recipient = recipient;
            this.Subject = subject;
            this.Body = body;
        }

        public string Sender { get; }

        public string Recipient { get; }

        public string Subject { get; }

        public string Body { get; }
    }
}