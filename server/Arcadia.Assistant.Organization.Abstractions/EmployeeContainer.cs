namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;

    public class EmployeeContainer
    {
        public EmployeeMetadata Metadata { get; }

        public IActorRef Actor { get; }

        public EmployeeContainer(EmployeeMetadata metadata, IActorRef actor)
        {
            this.Metadata = metadata;
            this.Actor = actor;
        }
    }
}