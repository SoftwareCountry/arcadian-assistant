namespace Arcadia.Assistant.Calendar.SickLeave
{
    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Configuration.Configuration;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using MimeKit;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions;

    public class SendEmailSickLeaveActor : UntypedActor
    {
        private readonly IEmailSettings mailConfig;
        private readonly ISmtpSettings smtpConfig;
        private readonly ILoggingAdapter logger = Context.GetLogger();

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
                case SendNotification notification when notification.Employee == null:
                    break;

                case SendNotification notification:
                    this.SendEmail(notification);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendEmail(SendNotification notification)
        {
            using (var client = new SmtpClient())
            {
                this.logger.Debug("Sending a sick leave email notification for user {0}", notification.Employee.Name);
                var msg = this.CreateMimeMessage(this.GetNotificationText(notification));

                client.Connect(
                    this.smtpConfig.Host,
                    this.smtpConfig.Port,
                    this.smtpConfig.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                client.Authenticate(this.smtpConfig.User, this.smtpConfig.Password);
                client.Send(msg);
                client.Disconnect(true);
                this.logger.Debug("Sick leave email notification for user {0} was succesfully sent", notification.Employee.Name);
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

        private string GetNotificationText(SendNotification notification)
        {
            return string.Format(this.mailConfig.Body, notification.Employee.Name, notification.CalendarEvent.Dates.StartDate.ToString("D"));
        }

        public sealed class SendNotification
        {
            public EmployeeMetadata Employee { get; }

            public CalendarEvent CalendarEvent { get; }

            public SendNotification(EmployeeMetadata employee, CalendarEvent calendarEvent)
            {
                Employee = employee;
                this.CalendarEvent = calendarEvent;
            }
        }
    }
}
