namespace Arcadia.Assistant.Vacations.Contracts
{
    using System;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class VacationApproval
    {
        [DataMember]
        public EmployeeId ApproverId { get; set; }

        [DataMember]
        public DateTimeOffset? Timestamp { get; set; }

        [DataMember]
        public bool IsFinal { get; set; }

        public VacationApproval(EmployeeId approverId)
        {
            this.ApproverId = approverId;
        }
    }
}