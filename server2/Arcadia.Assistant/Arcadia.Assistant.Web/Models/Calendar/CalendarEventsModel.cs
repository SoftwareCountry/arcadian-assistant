namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;

    using NJsonSchema.Annotations;

    [JsonSchemaProcessor(typeof(StatusSwaggerSchemaFilter))]
    public class CalendarEventsModel
    {
        [Required]
        [CalendarEventTypeValidator]
        public string Type { get; set; }

        [Required]
        public DatesPeriodModel Dates { get; set; }

        [Required]
        [CalendarEventStatusValidator]
        public string Status { get; set; }

        public CalendarEventsModel()
        {
        }

        public CalendarEventsModel(string type, DatesPeriodModel dates, string status)
        {
            this.Type = type;
            this.Dates = dates;
            this.Status = status;
        }
    }
}