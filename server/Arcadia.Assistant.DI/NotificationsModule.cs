namespace Arcadia.Assistant.DI
{
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications.Email;

    public class NotificationsModule : Module
    {
        private readonly ISmtpSettings smtpConfig;

        public NotificationsModule(ISmtpSettings smtpConfig)
        {
            this.smtpConfig = smtpConfig;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new EmailNotificationsActor(this.smtpConfig));
        }
    }
}