namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;

    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventTypeValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (value)
            {
                case string type when CalendarEventTypes.IsKnownType(type):
                    return ValidationResult.Success;
                case string type:
                    var validTypes = string.Join(", ", CalendarEventTypes.All);
                    return new ValidationResult($"Calendar event type `{type}` is not recognized. Must be one of the ${validTypes}");
                default:
                    return new ValidationResult("Type must be string");
            }
        }
    }
}