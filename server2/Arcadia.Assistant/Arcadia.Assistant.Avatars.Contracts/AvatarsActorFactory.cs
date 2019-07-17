namespace Arcadia.Assistant.Avatars.Contracts
{
    using Employees.Contracts;

    using Microsoft.ServiceFabric.Actors;
    using Microsoft.ServiceFabric.Actors.Client;

    public class AvatarsActorFactory : IAvatars
    {
        private readonly IActorProxyFactory actorProxyFactory;

        public AvatarsActorFactory(IActorProxyFactory actorProxyFactory)
        {
            this.actorProxyFactory = actorProxyFactory;
        }

        public IAvatar Get(EmployeeId employeeId)
        {
            return this.actorProxyFactory.CreateActorProxy<IAvatar>(new ActorId(employeeId.Value), serviceName: "AvatarActorService");
        }
    }
}