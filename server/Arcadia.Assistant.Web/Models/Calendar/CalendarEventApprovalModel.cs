namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventApprovalModel
    {
        public CalendarEventApprovalModel(string approverId)
        {
            this.ApproverId = approverId;
        }

        [DataMember]
        [Required]
        public string ApproverId { get; set; }
    }
}