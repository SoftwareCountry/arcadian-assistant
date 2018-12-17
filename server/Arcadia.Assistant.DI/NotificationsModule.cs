namespace Arcadia.Assistant.DI
{
    using Autofac;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Notifications.Email;
    using Arcadia.Assistant.Notifications.Push;

    public class NotificationsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<EmailNotificationsActor>().AsSelf();
            builder.RegisterType<PushNotificationsActor>().AsSelf();
        }
    }
}