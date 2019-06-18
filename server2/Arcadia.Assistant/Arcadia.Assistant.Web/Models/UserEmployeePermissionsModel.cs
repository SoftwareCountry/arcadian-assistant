namespace Arcadia.Assistant.Web.Models
{
    using System;
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

    [Flags]
    public enum EmployeePermissionsEntry
    {
        None = 0,
        ReadEmployeeInfo = 1 << 0,
        ReadEmployeeVacationsCounter = 1 << 1,
        ReadEmployeeDayoffsCounter = 1 << 2,
        ReadEmployeePhone = 1 << 3,
        ReadEmployeeCalendarEvents = 1 << 4,
        CreateCalendarEvents = 1 << 5,
        ApproveCalendarEvents = 1 << 6,
        RejectCalendarEvents = 1 << 7,
        CompleteSickLeave = 1 << 8,
        ProlongSickLeave = 1 << 9,
        CancelPendingCalendarEvents = 1 << 10,
        EditPendingCalendarEvents = 1 << 11,
        CancelApprovedCalendarEvents = 1 << 12
    }
}