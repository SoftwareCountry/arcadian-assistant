namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventApprovalModel
    {
        public CalendarEventApprovalModel(string approverId, DateTimeOffset? timestamp)
        {
            this.ApproverId = approverId;
            this.Timestamp = timestamp;
        }

        [DataMember]
        public string ApproverId { get; }

        [DataMember]
        public DateTimeOffset? Timestamp { get; }
    }
}