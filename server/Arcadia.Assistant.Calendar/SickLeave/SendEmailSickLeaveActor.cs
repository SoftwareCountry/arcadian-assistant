using Arcadia.Assistant.Calendar.SickLeave.Events;
using System;
using Akka.Actor;
using Akka.DI.Core;
using MimeKit;

namespace Arcadia.Assistant.Calendar.SickLeave
{
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Microsoft.Extensions.Configuration;

    public class SendEmailSickLeaveActor : UntypedActor
    {
        private readonly IConfigurationSection mailConfig;
        public SendEmailSickLeaveActor(IConfigurationSection mailConfig)
        {
            this.mailConfig = mailConfig;
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
            var user = mailConfig["SmtpUser"];
            var pass = mailConfig["SmtpPass"];

            using (var client = new SmtpClient())
            {
                var body = "Dear Sir/Madam,\nNotify you, that employee " + Event.UserId +
                    @" went on sick leave for " +
                    Event.TimeStamp.ToString() + "\nYours faithfully,\nTesting team\n\n";
                var message = CreateMimeMessage("Approved sick leave", body);

                client.Connect("smtp.gmail.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate(user, pass);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public MimeMessage CreateMimeMessage(String Subject, String Body)
        {
            var senderName = mailConfig["EmailSenderName"];
            var senderAddress = mailConfig["EmailSenderAddress"];
            var receiverName = mailConfig["EmailReceiverName"];
            var receiverAddress = mailConfig["EmailReceiverAddress"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(senderName, senderAddress));
            message.To.Add(new MailboxAddress(receiverName, receiverAddress));
            message.Subject = Subject;
            message.Body = new TextPart("plain") { Text = Body };

            return message;
        }
    }
}
