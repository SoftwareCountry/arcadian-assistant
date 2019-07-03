namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkHoursChangeRequest
    {
        [DataMember]
        public WorkHoursChange Change { get; set; }

        [DataMember]
        public RequestStatusChange[] RequestStatusChanges { get; set; }

        [DataContract]
        public class RequestStatusChange
        {
            [DataMember]
            public DateTimeOffset TimeSpan { get; set; }

            [DataMember]
            public string EmployeeId { get; set; }

            [DataMember]
            public WorkHoursChangeType Status { get; set; }
        }
    }
}