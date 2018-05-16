using Arcadia.Assistant.Calendar.SickLeave.Events;
using System;
using System.Configuration;
using System.Threading;
using Akka.Actor;
using MimeKit;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Gmail.v1.Data;

namespace Arcadia.Assistant.Calendar.SickLeave
{
    public class SendEmailSickLeaveActor : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SickLeaveIsApproved ev:
                    SendEmail(ev);
                    break;
            }
        }

        private static void SendEmail(SickLeaveIsApproved Event)
        {
            var clientId = ConfigurationManager.AppSettings["ClientId"];
            var clientSecret = ConfigurationManager.AppSettings["ClientSecret"];
            try
            {
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                    },
                    new[] { GmailService.Scope.GmailCompose },
                    "user",
                    CancellationToken.None).Result;

                var service = new GmailService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Gmail API",
                });
                var body = "Dear Sir/Madam,\nNotify you, that employee " + Event.UserId + @" went on sick leave for " +
                           Event.TimeStamp.ToString() + "\nYours faithfully,\nTesting team\n\n";
                service.Users.Messages.Send(createMessageFromMime(CreateMimeMessage("Approved sick leave", body)), "me").Execute();
            }
            catch (Exception e)
            {
                throw new Exception("Can't send email");
            }
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        public static Message createMessageFromMime(MimeMessage emailContent)
        {
            try
            {
                var encodedText = Base64UrlEncode(emailContent.ToString());
                var message = new Message { Raw = encodedText };
                return message;
            }
            catch (Exception e)
            {
                throw new Exception("Problems with buffer during sending email");
            }
        }

        public static MimeMessage CreateMimeMessage(String Subject, String Body)
        {
            var senderName = ConfigurationManager.AppSettings["EmailSenderName"];
            var senderAddress = ConfigurationManager.AppSettings["EmailSenderAddress"];
            var receiverName = ConfigurationManager.AppSettings["EmailReceiverName"];
            var receiverAddress = ConfigurationManager.AppSettings["EmailReceiverAddress"];

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(senderName, senderAddress));
                message.To.Add(new MailboxAddress(receiverName, receiverAddress));
                message.Subject = Subject;
                message.Body = new TextPart("plain") { Text = Body };

                return message;
            }
            catch (Exception e)
            {
                throw new Exception("Problems with creating mime message during sending email");
            }
        }
    }
}
