namespace Arcadia.Assistant.Web.Models
{
    using System.Linq;
    using System.Runtime.Serialization;

    using Permissions.Contracts;

    [DataContract]
    public class UserEmployeePermissionsModel
    {
        public UserEmployeePermissionsModel(string employeeId, EmployeePermissionsEntry employeePermissionsEntry)
        {
            this.EmployeeId = employeeId;
            this.PermissionsNames = this.ExtractPermissionNames(employeePermissionsEntry);
        }

        [DataMember]
        public string EmployeeId { get; }

        [DataMember]
        public string[] PermissionsNames { get; }

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