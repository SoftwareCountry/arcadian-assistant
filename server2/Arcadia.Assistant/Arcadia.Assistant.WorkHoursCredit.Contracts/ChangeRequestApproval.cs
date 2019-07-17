namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class ChangeRequestApproval
    {
        [DataMember]
        public DateTimeOffset Timestamp { get; set; }

        [DataMember]
        public EmployeeId ApproverId { get; set; }
    }
}