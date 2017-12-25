namespace Arcadia.Assistant.Organization.Abstractions
{
    using Akka.Actor;

    public class DepartmentContainer
    {
        public DepartmentInfo Department { get; }

        public IActorRef DepartmentActor { get; }

        public DepartmentContainer(DepartmentInfo department, IActorRef departmentActor)
        {
            this.Department = department;
            this.DepartmentActor = departmentActor;
        }
    }
}