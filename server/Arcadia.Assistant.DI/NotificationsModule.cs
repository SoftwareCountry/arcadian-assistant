namespace Arcadia.Assistant.DI
{
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Notifications.Push;

    public class NotificationsModule : Module
    {
        private readonly ISmtpSettings smtpSettings;
        private readonly IPushSettings pushSettings;

        public NotificationsModule(ISmtpSettings smtpSettings, IPushSettings pushSettings)
        {
            this.smtpSettings = smtpSettings;
            this.pushSettings = pushSettings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(x => new EmailNotificationsActor(this.smtpSettings));
            builder.Register(x => new PushNotificationsActor(this.pushSettings));
        }
    }
}