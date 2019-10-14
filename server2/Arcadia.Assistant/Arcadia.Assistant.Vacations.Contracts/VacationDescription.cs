namespace Arcadia.Assistant.Vacations.Contracts
{
    using System;
    using System.Runtime.Serialization;

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
        public string CancellationReason { get; set; }
    }
}