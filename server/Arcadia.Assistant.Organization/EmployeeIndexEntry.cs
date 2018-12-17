namespace Arcadia.Assistant.Organization
{

    using Akka.Actor;
    using Arcadia.Assistant.Organization.Abstractions;
    public class EmployeeIndexEntry
    {
        public IActorRef EmployeeActor { get; }

        public EmployeeMetadata Metadata { get; }

        public EmployeeIndexEntry(IActorRef employeeActor, EmployeeMetadata metadata)
        {
            this.EmployeeActor = employeeActor;
            this.Metadata = metadata;
        }
    }
}