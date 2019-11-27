namespace Arcadia.Assistant.Employees.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security.Principal;

    [DataContract]
    [KnownType(typeof(string[]))]
    public class EmployeesQuery
    {
        [DataMember]
        public IReadOnlyList<string>? DepartmentIds { get; private set; }

        [DataMember]
        public EmployeeId? EmployeeId { get; private set; }

        [DataMember]
        public string? RoomNumber { get; private set; }

        [DataMember]
        public string? Identity { get; private set; }

        [DataMember]
        public string? NameFilter { get; private set; }

        public static EmployeesQuery Create()
        {
            return new EmployeesQuery();
        }

        public EmployeesQuery ForDepartment(string departmentId)
        {
            var obj = this.Clone();
            obj.DepartmentIds = new[] { departmentId };
            return obj;
        }

        public EmployeesQuery ForDepartments(IEnumerable<string> departmentIds)
        {
            var obj = this.Clone();
            obj.DepartmentIds = departmentIds.ToArray();
            return obj;
        }

        public EmployeesQuery ForRoom(string roomNumber)
        {
            var obj = this.Clone();
            obj.RoomNumber = roomNumber;
            return obj;
        }

        public EmployeesQuery WithId(EmployeeId employeeId)
        {
            var obj = this.Clone();
            obj.EmployeeId = new EmployeeId(employeeId.Value);
            return obj;
        }

        public EmployeesQuery WithIdentity(string identity)
        {
            var obj = this.Clone();
            obj.Identity = identity;
            return obj;
        }

        public EmployeesQuery WithIdentity(IIdentity identity)
        {
            var obj = this.Clone();
            if (identity.Name == null)
            {
                throw new ArgumentException("Identity name is null");
            }

            obj.Identity = identity.Name;
            return obj;
        }

        public EmployeesQuery WithNameFilter(string nameFilter)
        {
            var obj = this.Clone();
            obj.NameFilter = nameFilter;
            return obj;
        }

        private EmployeesQuery Clone()
        {
            var newObj = new EmployeesQuery();
            newObj.DepartmentIds = this.DepartmentIds;
            newObj.EmployeeId = this.EmployeeId;
            newObj.RoomNumber = this.RoomNumber;
            //newObj.BirthDate = this.BirthDate;
            //newObj.HireDate = this.HireDate;
            newObj.Identity = this.Identity;
            newObj.NameFilter = this.NameFilter;

            return newObj;
        }
    }
}