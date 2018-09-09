namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Linq;
    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;

    using MimeKit;
    using Configuration.Configuration;
    using MailKit.Net.Smtp;
    using MailKit.Security;
    using Events;
    using Organization.Abstractions;
    using Organization.Abstractions.OrganizationRequests;

    public class SendEmailSickLeaveActor : UntypedActor
    {
        private readonly IEmailSettings mailConfig;
        private readonly ISmtpSettings smtpConfig;
        private readonly ILoggingAdapter logger = Context.GetLogger();
        private IActorRef employeesActor;

        public SendEmailSickLeaveActor(IEmailSettings mailConfig, ISmtpSettings smtpConfig)
        {
            this.mailConfig = mailConfig;
            this.smtpConfig = smtpConfig;
        }

        public static Props GetProps()
        {
            return Context.DI().Props<SendEmailSickLeaveActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case SendNotification notification:
                    if (this.employeesActor != null)
                    {
                        //TODO: Fix hardcode
                        this.employeesActor
                            .Ask<EmployeesQuery.Response>(new EmployeesQuery().WithId(notification.CalendarEvent.EmployeeId), TimeSpan.FromSeconds(10))
                            .ContinueWith(task => new SendNotificationEnd(task.Result, notification.CalendarEvent.Dates))
                            .PipeTo(this.Self);
                    }
                    else
                    {
                        this.logger.Warning($"Employees actor ref is not set in sending email actor. {Context.Self}");
                    }

                    break;

                //TODO: I hate this, remove...
                case SetEmployeesActor actor:
                    this.employeesActor = actor.EmployeesActor;
                    break;

                case SendNotificationEnd notification when notification.Employee == null:
                    break;

                case SendNotificationEnd notification:
                    this.SendEmail(notification);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SendEmail(SendNotificationEnd notification)
        {
            using (var client = new SmtpClient())
            {
                this.logger.Debug("Sending a sick leave email notification for user {0}", notification.Employee.Metadata.Name);
                var msg = this.CreateMimeMessage(this.GetNotificationText(notification));

                client.Connect(
                    this.smtpConfig.Host,
                    this.smtpConfig.Port,
                    this.smtpConfig.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                client.Authenticate(this.smtpConfig.User, this.smtpConfig.Password);
                client.Send(msg);
                client.Disconnect(true);
                this.logger.Debug("Sick leave email notification for user {0} was succesfully sent", notification.Employee.Metadata.Name);
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

        private string GetNotificationText(SendNotificationEnd notification)
        {
            return string.Format(this.mailConfig.Body, notification.Employee.Metadata.Name, notification.DatesPeriod.StartDate.ToString("D"));
        }

        private sealed class SendNotificationEnd
        {
            public SendNotificationEnd(EmployeesQuery.Response response, DatesPeriod datesPeriod)
            {
                this.DatesPeriod = datesPeriod;
                this.Employee = response.Employees.FirstOrDefault();
            }

            public DatesPeriod DatesPeriod { get; }

            public EmployeeContainer Employee { get; }
        }

        public sealed class SetEmployeesActor
        {
            public SetEmployeesActor(IActorRef emplActor)
            {
                this.EmployeesActor = emplActor;
            }

            public IActorRef EmployeesActor { get; }
        }

        public sealed class SendNotification
        {
            public CalendarEvent CalendarEvent { get; }

            public SendNotification(CalendarEvent calendarEvent)
            {
                this.CalendarEvent = calendarEvent;
            }
        }
    }
}
