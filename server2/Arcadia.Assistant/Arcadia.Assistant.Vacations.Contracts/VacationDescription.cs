namespace Arcadia.Assistant.Vacations.Contracts
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class VacationDescription
    {
        [DataMember]
        public int VacationId { get; set; }

        [DataMember]
        public EmployeeId EmployeeId { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public VacationApproval[] Approvals { get; set; } = new VacationApproval[0];

        [DataMember]
        public bool IsRejected { get; set; } = false;

        [DataMember]
        public bool IsCancelled { get; set; } = false;

        [DataMember]
        public bool IsProcessed { get; set; } = false;

        [DataMember]
        public bool AccountingReady { get; set; } = false;

        [DataMember]
        public string? CancellationReason { get; set; }

        public VacationStatus Status => 
            this.IsCancelled ? VacationStatus.Cancelled 
            : this.IsRejected ? VacationStatus.Rejected
            : this.IsProcessed ? VacationStatus.Processed
            : this.AccountingReady ? VacationStatus.AccountingReady
            : this.Approvals.Any(x => x.IsFinal) ? VacationStatus.Approved
            : VacationStatus.Requested;
    }
}