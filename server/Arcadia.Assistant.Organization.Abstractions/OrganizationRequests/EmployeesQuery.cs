namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class EmployeesQuery
    {
        [DataMember]
        private string[] departmentIds;

        public IReadOnlyCollection<string> DepartmentIds => this.departmentIds;

        [DataMember]
        public string AscendantDepartmentId { get; private set; }

        [DataMember]
        public string EmployeeId { get; private set; }

        [DataMember]
        public string RoomNumber { get; private set; }

        [DataMember]
        public string Sid { get; private set; }

        [DataMember]
        public string Email { get; private set; }

        [DataMember]
        public string DirectSupervisorId { get; private set; }

        [DataMember]
        public DateQuery BirthDate { get; private set; }

        [DataMember]
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
            obj.departmentIds = departmentIds.Distinct().ToArray();
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

        public EmployeesQuery SubordinateOf(string employeeId)
        {
            var obj = this.Clone();
            obj.DirectSupervisorId = employeeId;
            return obj;
        }

        private EmployeesQuery Clone()
        {
            var newObj = new EmployeesQuery();
            newObj.departmentIds = this.departmentIds;
            newObj.AscendantDepartmentId = this.AscendantDepartmentId;
            newObj.EmployeeId = this.EmployeeId;
            newObj.RoomNumber = this.RoomNumber;
            newObj.BirthDate = this.BirthDate;
            newObj.HireDate = this.HireDate;
            newObj.Sid = this.Sid;
            newObj.Email = this.Email;
            newObj.DirectSupervisorId = this.DirectSupervisorId;

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