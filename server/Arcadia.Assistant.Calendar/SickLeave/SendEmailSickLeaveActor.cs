using Arcadia.Assistant.Calendar.SickLeave.Events;
using System;
using Akka.Actor;
using Akka.DI.Core;
using MimeKit;

namespace Arcadia.Assistant.Calendar.SickLeave
{
    using Configuration.Configuration;
    using MailKit.Net.Smtp;
    using MailKit.Security;

    public class SendEmailSickLeaveActor : UntypedActor
    {
        private readonly IEmailSettings mailConfig;
        private readonly ISmtpSettings smtpConfig;
        public SendEmailSickLeaveActor(IEmailSettings mailConfig, ISmtpSettings smtpConfig)
        {
            this.mailConfig = mailConfig;
            this.smtpConfig = smtpConfig;
        }

        public static Props GetProps() => Context.DI().Props<SendEmailSickLeaveActor>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SickLeaveIsApproved ev:
                    SendEmail(ev);
                    break;
            }
        }

        private void SendEmail(SickLeaveIsApproved Event)
        {
            using (var client = new SmtpClient())
            {
                var message = CreateMimeMessage(String.Format(mailConfig.Body, Event.UserId, Event.TimeStamp.ToString()));

                client.Connect(smtpConfig.Host, smtpConfig.Port, SecureSocketOptions.StartTls);
                client.Authenticate(smtpConfig.User, smtpConfig.Password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public MimeMessage CreateMimeMessage(string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("From", mailConfig.NotificationSender));
            message.To.Add(new MailboxAddress("To", mailConfig.NotificationRecipient));
            message.Subject = mailConfig.Subject;
            message.Body = new TextPart("plain") { Text = body };

            return message;
        }
    }
}
