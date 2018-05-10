namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EmployeesQuery
    {
        public IReadOnlyCollection<string> DepartmentIds { get; private set; }

        public string AscendantDepartmentId { get; private set; }

        public string EmployeeId { get; private set; }

        public string RoomNumber { get; private set; }

        public string Sid { get; private set; }

        public string Email { get; private set; }

        public DateQuery BirthDate { get; private set; }

        public DateQuery HireDate { get; private set; }

        public static EmployeesQuery Create()
        {
            return new EmployeesQuery();
        }

        public EmployeesQuery SubordinateForDepartment(string departmentId)
        {
            var obj = this.Clone();
            obj.AscendantDepartmentId = departmentId;
            return obj;
        }

        public EmployeesQuery ForDepartments(params string[] departmentIds)
        {
            var obj = this.Clone();
            obj.DepartmentIds = departmentIds.Distinct().ToArray();
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

        public EmployeesQuery WithEmail(string email)
        {
            var obj = this.Clone();
            obj.Email = email;
            return obj;
        }

        public EmployeesQuery WithBirthDate(DateQuery bithDateQuery)
        {
            var obj = this.Clone();
            obj.BirthDate = bithDateQuery;
            return obj;
        }

        public EmployeesQuery WithHireDate(DateQuery hireDateQuery)
        {
            var obj = this.Clone();
            obj.HireDate = hireDateQuery;
            return obj;
        }

        private EmployeesQuery Clone()
        {
            var newObj = new EmployeesQuery();
            newObj.DepartmentIds = this.DepartmentIds;
            newObj.AscendantDepartmentId = this.AscendantDepartmentId;
            newObj.EmployeeId = this.EmployeeId;
            newObj.RoomNumber = this.RoomNumber;
            newObj.BirthDate = this.BirthDate;
            newObj.HireDate = this.HireDate;
            newObj.Sid = this.Sid;
            newObj.Email = this.Email;

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