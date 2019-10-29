namespace Arcadia.Assistant.Vacations.Contracts
{
    using System;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class VacationDescription
    {
        [DataMember]
        public int VacationId { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public VacationStatus Status { get; set; }

        [DataMember]
        public VacationApproval[] Approvals { get; set; } = new VacationApproval[0];

        [DataMember]
        public string? CancellationReason { get; set; }
    }
}