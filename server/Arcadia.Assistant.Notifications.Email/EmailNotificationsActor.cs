﻿namespace Arcadia.Assistant.Notifications.Email
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;

    public class EmailNotificationsActor : UntypedActor, ILogReceive
    {
        private readonly ISmtpSettings smtpSettings;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EmailNotificationsActor(ISmtpSettings smtpSettings)
        {
            this.smtpSettings = smtpSettings;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case EmailNotification msg when this.smtpSettings.Enabled:
                    this.SendEmail(msg);
                    break;
            }
        }

        private void SendEmail(EmailNotification message)
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

        private MimeMessage CreateMimeMessage(EmailNotification message)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(new MailboxAddress(message.Sender));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("plain") { Text = message.Body };

            foreach (var recipient in message.Recipients)
            {
                mimeMessage.To.Add(new MailboxAddress(recipient));
            }

            return mimeMessage;
        }
    }
}