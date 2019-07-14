﻿namespace Arcadia.Assistant.Permissions.Contracts
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class UserPermissionsCollection
    {
        [DataMember]
        // explicit department permissions by id
        private ReadOnlyDictionary<string, EmployeePermissionsEntry> DepartmentPermissions { get; set; }

        [DataMember]
        // explicit employees permissions by id
        private ReadOnlyDictionary<string, EmployeePermissionsEntry> EmployeePermissions { get; set; }

        [DataMember]
        private EmployeePermissionsEntry defaultPermission;

        public UserPermissionsCollection(
            EmployeePermissionsEntry defaultPermission,
            IDictionary<string, EmployeePermissionsEntry> departmentPermissions,
            IDictionary<string, EmployeePermissionsEntry> employeePermissions)
        {
            this.defaultPermission = defaultPermission;
            this.DepartmentPermissions = new ReadOnlyDictionary<string, EmployeePermissionsEntry>(departmentPermissions);
            this.EmployeePermissions = new ReadOnlyDictionary<string, EmployeePermissionsEntry>(employeePermissions);
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

            if (this.DepartmentPermissions.TryGetValue(objectEmployee.DepartmentId, out var departmentPermissions))
            {
                return permissions | departmentPermissions;
            }

            return permissions;
        }
    }
}