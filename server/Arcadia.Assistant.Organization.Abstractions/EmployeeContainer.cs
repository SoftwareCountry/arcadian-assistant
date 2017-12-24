namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;

    public class EmployeeContainer
    {
        public EmployeeStoredInformation EmployeeStoredInformation { get; }

        public IActorRef Actor { get; }

        public EmployeeContainer(EmployeeStoredInformation employeeStoredInformation, IActorRef actor)
        {
            this.EmployeeStoredInformation = employeeStoredInformation;
            this.Actor = actor;
        }
    }
}