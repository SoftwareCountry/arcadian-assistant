namespace Arcadia.Assistant.Permissions.Contracts
{
    using Employees.Contracts;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserPermissionsCollection
    {
        [DataMember]
        // explicit department permissions by id
        private ReadOnlyDictionary<DepartmentId, EmployeePermissionsEntry> DepartmentPermissions { get; set; }

        [DataMember]
        // explicit employees permissions by id
        private ReadOnlyDictionary<EmployeeId, EmployeePermissionsEntry> EmployeePermissions { get; set; }

        [DataMember]
        private EmployeePermissionsEntry defaultPermission;

        public UserPermissionsCollection(
            EmployeePermissionsEntry defaultPermission,
            IDictionary<DepartmentId, EmployeePermissionsEntry> departmentPermissions,
            IDictionary<EmployeeId, EmployeePermissionsEntry> employeePermissions)
        {
            this.defaultPermission = defaultPermission;
            this.DepartmentPermissions = new ReadOnlyDictionary<DepartmentId, EmployeePermissionsEntry>(departmentPermissions);
            this.EmployeePermissions = new ReadOnlyDictionary<EmployeeId, EmployeePermissionsEntry>(employeePermissions);
        }


        public EmployeePermissionsEntry GetPermissions(EmployeeMetadata objectEmployee)
        {
            if (objectEmployee == null)
            {
                return EmployeePermissionsEntry.None;
            }

            var permissions = this.defaultPermission;
            if (this.EmployeePermissions.TryGetValue(objectEmployee.EmployeeId, out var employeePermissions))
            {
                return permissions | employeePermissions;
            }

            if (objectEmployee.DepartmentId.HasValue
                && this.DepartmentPermissions.TryGetValue(objectEmployee.DepartmentId.Value, out var departmentPermissions))
            {
                return permissions | departmentPermissions;
            }

            return permissions;
        }
    }
}