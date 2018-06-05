namespace Arcadia.Assistant.DI
{
    using System;
    using Autofac;
    using Calendar.SickLeave;
    using Calendar.SickLeave.Events;
    using Configuration.Configuration;

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
            builder.Register(x => new SendEmailSickLeaveActor(mailConfig, smtpConfig));
        }
    }
}
