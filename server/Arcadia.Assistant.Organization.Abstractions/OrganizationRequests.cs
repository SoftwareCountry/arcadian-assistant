namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;

    using Akka.Actor;

    public static class OrganizationRequests
    {
        public sealed class RequestEmployeeInfo
        {
            public string EmployeeId { get; }

            public RequestEmployeeInfo(string employeeId)
            {
                this.EmployeeId = employeeId;
            }

            public sealed class Success
            {
                public Success(EmployeeInfo employeeInfo)
                {
                    this.EmployeeInfo = employeeInfo;
                }

                public EmployeeInfo EmployeeInfo { get; }
            }

            public sealed class EmployeeNotFound
            {
                public EmployeeNotFound(string employeeId)
                {
                    this.EmployeeId = employeeId;
                }

                public string EmployeeId { get; }
            }
        }

        public sealed class RequestDepartments
        {
            public static readonly RequestDepartments Instance = new RequestDepartments();

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
}