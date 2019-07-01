namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    public class EmployeesQuery
    {
        public string DepartmentId { get; private set; }

        public string EmployeeId { get; private set; }

        public string RoomNumber { get; private set; }

        public string Sid { get; private set; }

        public string Identity { get; private set; }

        public string NameFilter { get; private set; }

        public string DirectSupervisorId { get; private set; }

        public DateQuery BirthDate { get; private set; }

        public DateQuery HireDate { get; private set; }

        public bool? IsWorking { get; private set; }

        public static EmployeesQuery Create()
        {
            return new EmployeesQuery();
        }

        public EmployeesQuery ForDepartment(string departmentId)
        {
            var obj = this.Clone();
            obj.DepartmentId = departmentId;
            return obj;
        }

        public EmployeesQuery ForRoom(string roomNumber)
        {
            var obj = this.Clone();
            obj.RoomNumber = roomNumber;
            return obj;
        }

        public EmployeesQuery WithId(string employeeId)
        {
            var obj = this.Clone();
            obj.EmployeeId = employeeId;
            return obj;
        }

        public EmployeesQuery WithSid(string sid)
        {
            var obj = this.Clone();
            obj.Sid = sid;
            return obj;
        }

        public EmployeesQuery WithIdentity(string identity)
        {
            var obj = this.Clone();
            obj.Identity = identity;
            return obj;
        }

        public EmployeesQuery WithNameFilter(string nameFilter)
        {
            var obj = this.Clone();
            obj.NameFilter = nameFilter;
            return obj;
        }

        public EmployeesQuery WithBirthDate(DateQuery birthDateQuery)
        {
            var obj = this.Clone();
            obj.BirthDate = birthDateQuery;
            return obj;
        }

        public EmployeesQuery WithHireDate(DateQuery hireDateQuery)
        {
            var obj = this.Clone();
            obj.HireDate = hireDateQuery;
            return obj;
        }

        public EmployeesQuery WorkingOnly()
        {
            var obj = this.Clone();
            obj.IsWorking = true;
            return obj;
        }

        public EmployeesQuery SubordinateOf(string employeeId)
        {
            var obj = this.Clone();
            obj.DirectSupervisorId = employeeId;
            return obj;
        }

        private EmployeesQuery Clone()
        {
            var newObj = new EmployeesQuery
            {
                DepartmentId = this.DepartmentId,
                EmployeeId = this.EmployeeId,
                RoomNumber = this.RoomNumber,
                BirthDate = this.BirthDate,
                HireDate = this.HireDate,
                Sid = this.Sid,
                Identity = this.Identity,
                NameFilter = this.NameFilter,
                DirectSupervisorId = this.DirectSupervisorId,
                IsWorking = this.IsWorking
            };

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