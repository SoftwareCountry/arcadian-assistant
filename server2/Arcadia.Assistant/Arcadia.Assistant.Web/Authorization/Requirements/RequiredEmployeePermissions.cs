namespace Arcadia.Assistant.Web.Authorization.Requirements
{
    using Microsoft.AspNetCore.Authorization;

    using Permissions.Contracts;

    public class RequiredEmployeePermissions : IAuthorizationRequirement
    {
        public EmployeePermissionsEntry RequiredPermissions { get; private set; }

        public RequiredEmployeePermissions(EmployeePermissionsEntry requiredPermissions)
        {
            this.RequiredPermissions = requiredPermissions;
        }
    }
}