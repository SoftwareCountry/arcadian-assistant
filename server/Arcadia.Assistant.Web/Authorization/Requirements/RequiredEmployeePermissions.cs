namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Arcadia.Assistant.Security;

    using Microsoft.AspNetCore.Authorization;
    public class RequiredEmployeePermissions : IAuthorizationRequirement
    {
        public EmployeePermissionsEntry RequiredPermissions { get; private set; }

        public RequiredEmployeePermissions(EmployeePermissionsEntry requiredPermissions)
        {
            this.RequiredPermissions = requiredPermissions;
        }

        public virtual bool HasPermissions(EmployeePermissionsEntry employeePermissionsEntry)
        {
            return employeePermissionsEntry.HasFlag(this.RequiredPermissions);
        }
    }
}