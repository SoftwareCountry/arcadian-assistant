namespace Arcadia.Assistant.Notifications.Email
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;

    public class EmailNotificationsActor : UntypedActor, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly ISmtpSettings smtpSettings;

        public EmailNotificationsActor(ISmtpSettings smtpSettings)
        {
            this.smtpSettings = smtpSettings;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case EmailNotificationMessage msg:
                    this.SendEmail(msg);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendEmail(EmailNotificationMessage message)
        {
            this.logger.Debug("Email notification message received");

            using (var client = new SmtpClient())
            {
                var msg = this.CreateMimeMessage(message);

                client.Connect(
                    this.smtpSettings.Host,
                    this.smtpSettings.Port,
                    this.smtpSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                client.Authenticate(this.smtpSettings.User, this.smtpSettings.Password);
                client.Send(msg);
                client.Disconnect(true);
            }

            this.logger.Debug("Email was succesfully sent");
        }

        private MimeMessage CreateMimeMessage(EmailNotificationMessage message)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress("From", message.Sender));
            mimeMessage.To.Add(new MailboxAddress("To", message.Recipient));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("plain") { Text = message.Body };

            return mimeMessage;
        }
    }
}