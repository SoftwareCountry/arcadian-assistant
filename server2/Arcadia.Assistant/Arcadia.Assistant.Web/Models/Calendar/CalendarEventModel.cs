namespace Arcadia.Assistant.Web.Models.Calendar
{
    using NJsonSchema.Annotations;
    using System.ComponentModel.DataAnnotations;
    using System.Runtime.Serialization;

    [JsonSchemaProcessor(typeof(CalendarEventSchemaProcessor))]
    [WorkHoursChangeValidator]
    [DataContract]
    public class CalendarEventModel
    {
        [Required]
        [CalendarEventTypeValidator]
        [DataMember]
        public string Type { get; set; } = string.Empty;

        [Required]
        [DataMember]
        public DatesPeriodModel Dates { get; set; } = new DatesPeriodModel();

        [Required]
        [CalendarEventStatusValidator]
        [DataMember]
        public string Status { get; set; } = string.Empty;

        public CalendarEventModel()
        {
        }

        public CalendarEventModel(string type, DatesPeriodModel dates, string status)
        {
            this.Type = type;
            this.Dates = dates;
            this.Status = status;
        }
    }
}