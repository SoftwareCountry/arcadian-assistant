namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;
    using Akka.Util.Internal;
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
                case SickLeaveEmail email:
                    this.SendEmail(email);
                    break;

                case SickLeaveIsApproved ev:
                    if (this.employeesActor != null)
                    {
                        this.employeesActor
                            .Ask(new EmployeesQuery().WithId(ev.UserId))
                            .ContinueWith(result => new SickLeaveEmail(ev.TimeStamp, result.Result.AsInstanceOf<EmployeesQuery.Response>()), TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                            .PipeTo(this.Self);
                    }
                    else
                    {
                        this.logger.Warning($"Employees actor ref is not set in sending email actor. {Context.Self}");
                    }

                    break;

                case SetEmployeesActor actor:
                    this.employeesActor = actor.EmployeesActor;
                    break;
            }
        }

        private void SendEmail(SickLeaveEmail email)
        {
            using (var client = new SmtpClient())
            {
                this.logger.Debug("Sending a sick leave email notification for user {0}", email.GetEmployeeId());
                var msg = this.CreateMimeMessage(email.GetBody(this.mailConfig.Body));

                client.Connect(
                    this.smtpConfig.Host,
                    this.smtpConfig.Port,
                    this.smtpConfig.UseTls ? SecureSocketOptions.StartTls 
                                                                        : SecureSocketOptions.SslOnConnect);
                client.Authenticate(this.smtpConfig.User, this.smtpConfig.Password);
                client.Send(msg);
                client.Disconnect(true);
                this.logger.Debug("Sick leave email notification for user {0} was succesfully sent", email.GetEmployeeId());
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

        public sealed class SickLeaveEmail
        {
            public SickLeaveEmail(DateTimeOffset timeStamp, EmployeesQuery.Response response)
            {
                this.TimeStamp = timeStamp;
                this.Employee = response.Employees.FirstOrDefault();
            }

            public string GetBody(string format)
            {
                return string.Format(format, this.Employee.Metadata.Name, this.TimeStamp.ToString());
            }

            public string GetEmployeeId()
            {
                return this.Employee.Metadata.EmployeeId;
            }

            private DateTimeOffset TimeStamp { get; }

            private EmployeeContainer Employee { get; }
        }

        public sealed class SetEmployeesActor
        {
            public SetEmployeesActor(IActorRef emplActor)
            {
                this.EmployeesActor = emplActor;
            }

            public IActorRef EmployeesActor { get; }
        }
    }
}
