namespace Arcadia.Assistant.UserPreferences.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    public class UserPreferencesActorFactory : IUsersPreferences
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public UserPreferencesActorFactory(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        public IUserPreferences Get(string userId)
        {
            var actor = this.actorProxyFactory.CreateActorProxy<IUserPreferences>(new ActorId(userId), serviceName: "UserPreferencesActorService");
            return actor;
        }
    }
}