namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System;

    public class Approval
    {
        public Approval(DateTimeOffset timestamp, string approvedBy)
        {
            this.Timestamp = timestamp;
            this.ApprovedBy = approvedBy;
        }

        public DateTimeOffset Timestamp { get; set; }

        public string ApprovedBy { get; set; }
    }
}