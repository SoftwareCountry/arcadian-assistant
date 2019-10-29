namespace Arcadia.Assistant.PendingActions.Contracts
{
    using System.Runtime.Serialization;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    [DataContract]
    public class PendingRequests
    {
        [DataMember]
        public VacationDescription[] PendingVacations { get; set; } = new VacationDescription[0];

        [DataMember]
        public WorkHoursChangeRequest[] PendingWorkHoursChanges { get; set; } = new WorkHoursChangeRequest[0];
    }
}