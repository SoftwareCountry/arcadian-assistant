namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [DataContract]
    public class CalendarEventWithIdModel : CalendarEventModel
    {
        [Required]
        [DataMember]
        public string? CalendarEventId { get; set; }

        public CalendarEventWithIdModel()
        {
        }

        public CalendarEventWithIdModel(string calendarEventId, string type, DatesPeriodModel dates, string status)
            : base(type, dates, status)
        {
            this.CalendarEventId = calendarEventId;
        }
    }
}