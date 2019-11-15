﻿namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Runtime.Serialization;

    using Employees.Contracts;

    [DataContract]
    public class WorkHoursChange
    {
        [DataMember]
        public EmployeeId EmployeeId { get; set; }

        [DataMember]
        public Guid ChangeId { get; set; }

        [DataMember]
        public WorkHoursChangeType ChangeType { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public DayPart DayPart { get; set; }

        [DataMember]
        public ChangeRequestStatus Status { get; set; }
    }
}