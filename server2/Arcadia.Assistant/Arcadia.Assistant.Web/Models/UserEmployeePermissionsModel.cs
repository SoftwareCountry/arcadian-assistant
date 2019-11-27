namespace Arcadia.Assistant.Web.Models
{
    using Permissions.Contracts;
    using System.Linq;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserEmployeePermissionsModel
    {
        [DataMember]
        public string EmployeeId { get; }

        [DataMember]
        public string[] PermissionsNames { get; }

        public UserEmployeePermissionsModel(string employeeId, EmployeePermissionsEntry employeePermissionsEntry)
        {
            this.EmployeeId = employeeId;
            this.PermissionsNames = this.ExtractPermissionNames(employeePermissionsEntry);
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