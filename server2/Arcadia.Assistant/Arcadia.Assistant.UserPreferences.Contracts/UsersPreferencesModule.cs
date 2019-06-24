namespace Arcadia.Assistant.UserPreferences.Contracts
{
    using Autofac;

    public class UsersPreferencesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<UserPreferencesActorFactory>().As<IUsersPreferencesStorage>();
        }
    }
}