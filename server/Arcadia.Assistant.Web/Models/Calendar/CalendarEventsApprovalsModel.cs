namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventsApprovalsModel
    {
        public CalendarEventsApprovalsModel(IEnumerable<string> approvals)
        {
            this.Approvals = approvals;
        }

        [DataMember]
        public IEnumerable<string> Approvals { get; set; }
    }
}