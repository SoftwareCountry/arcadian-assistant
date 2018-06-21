namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Security;

    public class UserEmployeePermissionsModel
    {
        public string[] UserEmployeePermissions { get; }

        public UserEmployeePermissionsModel(EmployeePermissionsEntry employeePermissionsEntry)
        {
            this.UserEmployeePermissions = this.ExtractPermissionNames(employeePermissionsEntry);
        }

        private string[] ExtractPermissionNames(EmployeePermissionsEntry employeePermissionsEntry)
        {
            return employeePermissionsEntry
                .ToString("G")
                .Split(',')
                .Select(x =>
                {
                    var trimmed = x.Trim();
                    var camelCased = char.ToLowerInvariant(trimmed[0]) + trimmed.Substring(1);
                    return camelCased;
                })
                .ToArray();
        }
    }
}