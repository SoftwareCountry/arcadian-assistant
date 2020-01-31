namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;

    using Permissions.Contracts;

    public class EmployeePermissionsRequirement : IAuthorizationRequirement
    {
        public EmployeePermissionsEntry RequiredPermissions { get; private set; }

        public EmployeePermissionsRequirement(EmployeePermissionsEntry requiredPermissions)
        {
            this.RequiredPermissions = requiredPermissions;
        }
    }
}