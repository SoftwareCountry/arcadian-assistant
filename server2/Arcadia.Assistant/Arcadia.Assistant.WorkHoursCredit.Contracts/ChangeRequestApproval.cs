namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using Employees.Contracts;
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ChangeRequestApproval
    {
        [DataMember]
        public DateTimeOffset Timestamp { get; set; }

        [DataMember]
        public EmployeeId ApproverId { get; set; }
    }
}