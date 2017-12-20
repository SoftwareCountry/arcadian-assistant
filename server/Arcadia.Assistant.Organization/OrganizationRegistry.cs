namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions;

    public class OrganizationRegistry : UntypedActor
    {
        private IDictionary<string, IActorRef> departmentsById = new Dictionary<string, IActorRef>();

        private IDictionary<string, IActorRef> employeesById = new Dictionary<string, IActorRef>();

        private IDictionary<string, IDictionary<string, IActorRef>> employeesByDepartment = new Dictionary<string, IDictionary<string, IActorRef>>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case DepartmentAddedOrChanged department:
                    this.departmentsById[department.DepartmentId] = department.Actor;
                    break;

                case DepartmentRemoved department:
                    var departmentActor = this.departmentsById[department.DepartmentId];
                    if (department.Actor == departmentActor)
                    {
                        // means that it was not replaced by new actor, so we just delete it
                        this.departmentsById.Remove(department.DepartmentId);
                    }
                    break;

                case EmployeeAddedOrChanged employee:
                    this.employeesById[employee.Employee.EmployeeId] = employee.Actor;
                    Context.WatchWith(employee.Actor, new EmployeeRemoved(employee.Employee.EmployeeId, employee.Actor));
                    break;

                case EmployeeRemoved employee:
                    var employeeActor = this.employeesById[employee.EmployeeId];
                    if (employee.Actor == employeeActor)
                    {
                        // means that it was not replaced by new actor, so we just delete it
                        this.employeesById.Remove(employee.EmployeeId);
                    }
                    break;

                case EmployeesActor.FindEmployee request when this.employeesById.ContainsKey(request.EmployeeId):
                    this.Sender.Tell(new EmployeesActor.FindEmployee.Response(request.EmployeeId, this.employeesById[request.EmployeeId]));
                    break;
                case EmployeesActor.FindEmployee request:
                    this.Sender.Tell(new EmployeesActor.FindEmployee.Response(request.EmployeeId, Nobody.Instance));
                    break;
            }
        }

        public class DepartmentAddedOrChanged
        {
            public string DepartmentId { get; }

            public IActorRef Actor { get; }

            public DepartmentAddedOrChanged(string departmentId, IActorRef actor)
            {
                this.DepartmentId = departmentId;
                this.Actor = actor;
            }
        }

        private class DepartmentRemoved
        {
            public DepartmentRemoved(string departmentId, IActorRef actor)
            {
                this.DepartmentId = departmentId;
                this.Actor = actor;
            }

            public string DepartmentId { get; }

            public IActorRef Actor { get; }
        }

        public class EmployeeAddedOrChanged
        {
            public EmployeeInfo Employee { get; }

            public IActorRef Actor { get; }

            public EmployeeAddedOrChanged(EmployeeInfo employee, IActorRef actor)
            {
                this.Employee = employee;
                this.Actor = actor;
            }
        }

        public class EmployeeRemoved
        {
            public string EmployeeId { get; }

            public IActorRef Actor { get; }

            public EmployeeRemoved(string employeeId, IActorRef actor)
            {
                this.EmployeeId = employeeId;
                this.Actor = actor;
            }
        }
    }
}