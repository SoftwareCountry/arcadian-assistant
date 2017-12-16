namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    using Akka.Actor;

    public class FindEmployees
    {
        public sealed class Response
        {
            public IReadOnlyCollection<EmployeeFinding> Employees { get; }

            public Response(IReadOnlyCollection<EmployeeFinding> employees)
            {
                this.Employees = employees;
            }
        }

        public class EmployeeFinding
        {
            public EmployeeInfo EmployeeInfo { get; }

            public IActorRef Actor { get; }

            public EmployeeFinding(EmployeeInfo employeeInfo, IActorRef actor)
            {
                this.EmployeeInfo = employeeInfo;
                this.Actor = actor;
            }
        }
    }
}