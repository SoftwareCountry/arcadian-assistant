namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;

    using NJsonSchema.Annotations;

    [JsonSchemaProcessor(typeof(CalendarEventSchemaProcessor))]
    [WorkHoursChangeValidator]
    public class CalendarEventModel
    {
        [Required]
        [CalendarEventTypeValidator]
        public string Type { get; set; }

        [Required]
        public DatesPeriodModel Dates { get; set; }

        [Required]
        [CalendarEventStatusValidator]
        public string Status { get; set; }

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