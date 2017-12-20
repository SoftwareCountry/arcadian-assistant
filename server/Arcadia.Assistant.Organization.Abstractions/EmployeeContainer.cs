namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;

    public class EmployeeContainer
    {
        public EmployeeInfo EmployeeInfo { get; }

        public IActorRef Actor { get; }

        public EmployeeContainer(EmployeeInfo employeeInfo, IActorRef actor)
        {
            this.EmployeeInfo = employeeInfo;
            this.Actor = actor;
        }
    }
}