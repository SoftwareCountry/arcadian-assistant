namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;
    using System.Diagnostics;

    using Akka.Actor;

    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
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

        private string DebuggerDisplay 
            => $"#{this.Department?.DepartmentId} {this.Department?.Abbreviation}, Head {this.Head?.Metadata?.Name}, Employees count: {this.Employees?.Count}";
    }
}