namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Security;

    public class UserEmployeePermissionsModel
    {
        public string EmployeeId { get; }

        public string[] permissionsNames { get; }

        public UserEmployeePermissionsModel(string employeeId, EmployeePermissionsEntry employeePermissionsEntry)
        {
            this.EmployeeId = employeeId;
            this.permissionsNames = this.ExtractPermissionNames(employeePermissionsEntry);
        }

        private string[] ExtractPermissionNames(EmployeePermissionsEntry employeePermissionsEntry)
        {
            return employeePermissionsEntry
                .ToString("G")
                .Split(", ")
                .Select(x =>
                {
                    var camelCased = char.ToLowerInvariant(x[0]) + x.Substring(1);
                    return camelCased;
                })
                .ToArray();
        }
    }
}