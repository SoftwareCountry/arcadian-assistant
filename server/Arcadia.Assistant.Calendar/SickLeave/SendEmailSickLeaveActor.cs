namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using Akka.Event;

    using Configuration.Configuration;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Notifications.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions;

    public class SendEmailSickLeaveActor : BaseNotificationsActor
    {
        private readonly IEmailSettings mailConfig;
        private readonly ISmtpSettings smtpConfig;
        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SendEmailSickLeaveActor(IEmailSettings mailConfig, ISmtpSettings smtpConfig)
        {
            this.mailConfig = mailConfig;
            this.smtpConfig = smtpConfig;
        }

        protected override void HandleNotificationPayload(object payload)
        {
            switch (payload)
            {
                case SickLeaveNotification notification when notification.EmployeeName == null:
                    break;

                case SickLeaveNotification notification:
                    try
                    {
                        this.SendEmail(notification);
                    }
                    catch (Exception ex)
                    {
                        this.logger.Warning(ex.Message);
                    }

                    break;
            }
        }

        private void SendEmail(SickLeaveNotification notification)
        {
            using (var client = new SmtpClient())
            {
                this.logger.Debug("Sending a sick leave email notification for user {0}", notification.EmployeeName);
                var msg = this.CreateMimeMessage(this.GetNotificationText(notification));

                client.Connect(
                    this.smtpConfig.Host,
                    this.smtpConfig.Port,
                    this.smtpConfig.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                client.Authenticate(this.smtpConfig.User, this.smtpConfig.Password);
                client.Send(msg);
                client.Disconnect(true);
                this.logger.Debug("Sick leave email notification for user {0} was succesfully sent", notification.EmployeeName);
            }
        }

        private MimeMessage CreateMimeMessage(string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("From", this.mailConfig.NotificationSender));
            message.To.Add(new MailboxAddress("To", this.mailConfig.NotificationRecipient));
            message.Subject = this.mailConfig.Subject;
            message.Body = new TextPart("plain") { Text = body };

            return message;
        }

        private string GetNotificationText(SickLeaveNotification notification)
        {
            return string.Format(this.mailConfig.Body, notification.EmployeeName, notification.StartDate.ToString("D"));
        }

        public sealed class SickLeaveNotification
        {
            public SickLeaveNotification(string employeeName, DateTime startDate)
            {
                this.EmployeeName = employeeName;
                this.StartDate = startDate;
            }

            public string EmployeeName { get; }

            public DateTime StartDate { get; }
        }
    }
}
