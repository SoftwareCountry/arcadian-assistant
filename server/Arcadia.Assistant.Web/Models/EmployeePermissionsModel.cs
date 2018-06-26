namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Security;

    public class EmployeePermissionsModel
    {
        public string EmployeeId { get; }

        public string[] permissionsNames { get; }

        public EmployeePermissionsModel(string employeeId, EmployeePermissionsEntry employeePermissionsEntry)
        {
            this.EmployeeId = employeeId;
            this.permissionsNames = this.ExtractPermissionNames(employeePermissionsEntry);
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