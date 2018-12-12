namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventApprovalWithTimestampModel : CalendarEventApprovalModel
    {
        public CalendarEventApprovalWithTimestampModel(DateTimeOffset timestamp, string approverId)
            : base(approverId)
        {
            this.Timestamp = timestamp;
        }

        [DataMember]
        [Required]
        public DateTimeOffset Timestamp { get; }
    }
}