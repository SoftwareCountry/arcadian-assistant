namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using Employees.Contracts;
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkHoursChangeRequest
    {
        [DataMember]
        public WorkHoursChange Change { get; set; } = new WorkHoursChange();

        [DataMember]
        public RequestStatusChange[] RequestStatusChanges { get; set; } = new RequestStatusChange[0];

        [DataContract]
        public class RequestStatusChange
        {
            [DataMember]
            public DateTimeOffset TimeSpan { get; set; }

            [DataMember]
            public EmployeeId EmployeeId { get; set; }

            [DataMember]
            public WorkHoursChangeType Status { get; set; }
        }
    }
}