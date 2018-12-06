namespace Arcadia.Assistant.DI
{
    using Autofac;

    using Arcadia.Assistant.Calendar.SickLeave;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications.Email;

    public class NotificationsModule : Module
    {
        private readonly IEmailSettings mailConfig;
        private readonly ISmtpSettings smtpConfig;

        public NotificationsModule(ISmtpSettings smtpConfig, IEmailSettings mailConfig)
        {
            this.mailConfig = mailConfig;
            this.smtpConfig = smtpConfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new SendEmailSickLeaveActor(this.mailConfig));
            builder.Register(x => new EmailNotificationsActor(this.smtpConfig));
        }
    }
}