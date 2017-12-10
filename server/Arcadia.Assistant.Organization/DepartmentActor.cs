namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class DepartmentActor : UntypedActor
    {
        public class AssignHead
        {
            public IActorRef Employee { get; }

            public string EmployeeId { get; }

            public AssignHead(IActorRef employee, string employeeId)
            {
                this.Employee = employee;
                this.EmployeeId = employeeId;
            }
        }

        public class AssignEmployees
        {
            public IReadOnlyDictionary<string, IActorRef> Employees { get; }

            public AssignEmployees(IReadOnlyDictionary<string, IActorRef> employees)
            {
                this.Employees = employees;
            }
        }

        private readonly Department department;

        private string HeadEmployeeId { get; set; }

        private IActorRef Head { get; set; }

        public IReadOnlyDictionary<string, IActorRef> Empoyees { get; private set; }

        public DepartmentActor(Department department)
        {
            this.department = department;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AssignEmployees assignEmployees:
                    this.Empoyees = assignEmployees.Employees;
                    break;

                case AssignHead assignHead:
                    this.Head = assignHead.Employee;
                    this.HeadEmployeeId = assignHead.EmployeeId;
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}