namespace Arcadia.Assistant.Security
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Arcadia.Assistant.Organization.Abstractions;

    [DataContract]
    public class Permissions
    {

        [DataMember]
        // no matter what Department permissions say, those overrule everything
        private IReadOnlyDictionary<string, EmployeePermissionsEntry> UnconditionalEmployeePermissions { get; set; }

        [DataMember]
        // explicit department permissions by id
        private IReadOnlyDictionary<string, EmployeePermissionsEntry> DepartmentPermissions { get; set; }

        [DataMember]
        // explicit employees by id permissiosn
        private IReadOnlyDictionary<string, EmployeePermissionsEntry> EmployeePermissions { get; set; }

        [DataMember]
        private EmployeePermissionsEntry defaultPermission;

        public Permissions(
            EmployeePermissionsEntry defaultPermission,
            IReadOnlyDictionary<string, EmployeePermissionsEntry> unconditionalEmployeePermissions,
            IReadOnlyDictionary<string, EmployeePermissionsEntry> departmentPermissions,
            IReadOnlyDictionary<string, EmployeePermissionsEntry> employeePermissions)
        {
            this.defaultPermission = defaultPermission;
            this.UnconditionalEmployeePermissions = unconditionalEmployeePermissions;
            this.DepartmentPermissions = departmentPermissions;
            this.EmployeePermissions = employeePermissions;
        }


        public EmployeePermissionsEntry GetPermissions(EmployeeContainer targetEmployee)
        {
            if (targetEmployee == null)
            {
                return EmployeePermissionsEntry.None;
            }

            if (this.UnconditionalEmployeePermissions.TryGetValue(targetEmployee.Metadata.EmployeeId, out var unconditionalPermissions))
            {
                return unconditionalPermissions;
            }

            var permissions = this.defaultPermission;
            if (this.EmployeePermissions.TryGetValue(targetEmployee.Metadata.EmployeeId, out var employeePermissions))
            {
                permissions |= employeePermissions;
            }

            if (this.DepartmentPermissions.TryGetValue(targetEmployee.Metadata.DepartmentId, out var departmentPermissions))
            {
                permissions |= departmentPermissions;
            }

            return permissions;
        }
    }
}