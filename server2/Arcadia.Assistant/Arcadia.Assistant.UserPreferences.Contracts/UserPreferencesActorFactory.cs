namespace Arcadia.Assistant.UserPreferences.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    public class UserPreferencesActorFactory : IUsersPreferencesStorage
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public UserPreferencesActorFactory(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        public IUserPreferencesStorage ForEmployee(string employeeId)
        {
            var actor = this.actorProxyFactory.CreateActorProxy<IUserPreferencesStorage>(new ActorId("employee-" + employeeId), serviceName: "UserPreferencesStorageActorService");
            return actor;
        }
    }
}