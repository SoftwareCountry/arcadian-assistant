namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;

    public class EmployeeContainer
    {
        public EmployeeMetadata Metadata { get; }

        public IActorRef Actor { get; }

        public IActorRef Feed { get; }

        public IActorRef Calendar { get; }

        public EmployeeContainer(EmployeeMetadata metadata, IActorRef actor, IActorRef feed, IActorRef calendar)
        {
            this.Metadata = metadata;
            this.Actor = actor;
            this.Feed = feed;
            this.Calendar = calendar;
        }
    }
}