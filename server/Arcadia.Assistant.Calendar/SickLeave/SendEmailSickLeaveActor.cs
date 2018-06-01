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
        private IActorRef employeesActor = null;

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
                    SendEmail(email, true);
                    break;
                case SickLeaveIsApproved ev:
                    if (employeesActor != null)
                        employeesActor.Ask(new EmployeesQuery().WithId(ev.UserId))
                            .ContinueWith(result => new SickLeaveEmail(ev.TimeStamp, result.Result.AsInstanceOf<EmployeesQuery.Response>()),
                                TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                            .PipeTo(Self);
                    else
                        logger.Debug("Employees actor ref is not set in sending email actor");

                    break;
                case SetEmployeesActor actor:
                    employeesActor = actor.employeesActor;
                    break;
            }
        }

        private void SendEmail(SickLeaveEmail email, bool startTls)
        {
            using (var client = new SmtpClient())
            {
                logger.Debug("Sending a sick leave email notification for user {0}", email.GetEmployeeId());
                var msg = CreateMimeMessage(email.GetBody(mailConfig.Body));

                client.Connect(smtpConfig.Host, smtpConfig.Port, startTls ? SecureSocketOptions.StartTls 
                                                                        : SecureSocketOptions.SslOnConnect);
                client.Authenticate(smtpConfig.User, smtpConfig.Password);
                client.Send(msg);
                client.Disconnect(true);
                logger.Debug("Sick leave email notification for user {0} was succesfully sent", email.GetEmployeeId());
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

        public sealed class SickLeaveEmail
        {
            public SickLeaveEmail(DateTimeOffset timeStamp, EmployeesQuery.Response response)
            {
                this.timeStamp = timeStamp;
                employee = response.Employees.FirstOrDefault();
            }

            public string GetBody(string format)
            {
                return string.Format(format, employee.Metadata.Name, timeStamp.ToString());
            }

            public string GetEmployeeId()
            {
                return employee.Metadata.EmployeeId;
            }

            private DateTimeOffset timeStamp { get; }
            private EmployeeContainer employee { get; }
        }

        public sealed class SetEmployeesActor
        {
            public SetEmployeesActor(IActorRef emplActor)
            {
                employeesActor = emplActor;
            }

            public IActorRef employeesActor { get; }
        }
    }
}
