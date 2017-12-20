namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    using Akka.Actor;

    public class EmployeesQuery
    {
        public string DepartmentId { get; private set; }

        public string EmployeeId { get; private set; }

        public EmployeesQuery()
        {
        }

        public EmployeesQuery ForDepartment(string departmentId)
        {
            var obj = this.Clone();
            obj.DepartmentId = departmentId;
            return obj;
        }

        public EmployeesQuery WithId(string employeeId)
        {
            var obj = this.Clone();
            obj.EmployeeId = employeeId;
            return obj;
        }

        private EmployeesQuery Clone()
        {
            var newObj = new EmployeesQuery();
            newObj.DepartmentId = this.DepartmentId;
            newObj.EmployeeId = this.EmployeeId;

            return newObj;
        }

        public sealed class Response
        {
            public IReadOnlyCollection<EmployeeContainer> Employees { get; }

            public Response(IReadOnlyCollection<EmployeeContainer> employees)
            {
                this.Employees = employees;
            }
        }
    }
}