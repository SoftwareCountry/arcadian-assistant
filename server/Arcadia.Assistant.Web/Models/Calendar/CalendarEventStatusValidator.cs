namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventStatusValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var statuses = new CalendarEventStatuses();

            switch (validationContext.ObjectInstance)
            {
                case CalendarEventsModel model when statuses.AllForType(model.Type).Contains(value):
                    return ValidationResult.Success;
                case CalendarEventsModel model:
                    var validTypes = string.Join(", ", statuses.AllForType(model.Type));
                    return new ValidationResult($"Calendar event status `{value}` is not recognized. Must be one of the {validTypes}");
                default:
                    return new ValidationResult($"Attribute usage error: ValidationContext must be applied to {typeof(CalendarEventsModel)}");
            }
        }
    }
}