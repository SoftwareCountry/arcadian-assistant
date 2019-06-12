namespace Arcadia.Assistant.Organization.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class EmployeesQuery
    {
        [DataMember]
        public string DepartmentId { get; private set; }

        [DataMember]
        public string EmployeeId { get; private set; }

        [DataMember]
        public string RoomNumber { get; private set; }

        [DataMember]
        public string Identity { get; private set; }

        [DataMember]
        public string NameFilter { get; private set; }

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

        private EmployeesQuery Clone()
        {
            var newObj = new EmployeesQuery();
            newObj.DepartmentId = this.DepartmentId;
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