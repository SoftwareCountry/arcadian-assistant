namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventWithIdModel : CalendarEventModel
    {
        public CalendarEventWithIdModel()
        {
        }

        public CalendarEventWithIdModel(string calendarEventId, string type, DatesPeriodModel dates, string status)
            : base(type, dates, status)
        {
            this.CalendarEventId = calendarEventId;
        }

        [Required]
        [DataMember]
        public string? CalendarEventId { get; set; }
    }
}