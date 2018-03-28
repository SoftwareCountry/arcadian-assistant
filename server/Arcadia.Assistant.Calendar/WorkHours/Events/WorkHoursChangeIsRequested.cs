namespace Arcadia.Assistant.Calendar.WorkHours.Events
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class WorkHoursChangeIsRequested
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string EmployeeId { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public int StartHour { get; set; }

        [DataMember]
        public int EndHour { get; set; }

        [DataMember]
        public bool IsDayoff { get; set; }

        public bool IsWorkout() => !this.IsDayoff;
    }
}