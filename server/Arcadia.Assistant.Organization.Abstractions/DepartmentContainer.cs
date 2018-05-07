namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;

    using Akka.Actor;

    public class DepartmentContainer
    {
        public DepartmentInfo Department { get; }

        public IActorRef DepartmentActor { get; }

        public EmployeeContainer Head { get; }

        public IReadOnlyCollection<EmployeeContainer> Employees { get; }

        public DepartmentContainer(DepartmentInfo department, IActorRef departmentActor, EmployeeContainer head, IReadOnlyCollection<EmployeeContainer> employees)
        {
            this.Department = department;
            this.DepartmentActor = departmentActor;
            this.Head = head;
            this.Employees = employees;
        }
    }
}