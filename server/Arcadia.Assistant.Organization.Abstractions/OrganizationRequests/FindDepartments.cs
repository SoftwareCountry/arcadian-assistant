namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    using Akka.Actor;

    public sealed class FindDepartments
    {
        public sealed class Response
        {
            public IReadOnlyCollection<DepartmentFinding> Departments { get; }

            public Response(IReadOnlyCollection<DepartmentFinding> departments)
            {
                this.Departments = departments;
            }
        }


        public class DepartmentFinding
        {
            public Department Department { get; }

            public IActorRef DepartmentActor { get; }

            public IActorRef EmployeesActor { get; }

            public DepartmentFinding(Department department, IActorRef departmentActor, IActorRef employeesActor)
            {
                this.Department = department;
                this.DepartmentActor = departmentActor;
                this.EmployeesActor = employeesActor;
            }
        }
    }
}