namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;

    using Arcadia.Assistant.Calendar.Abstractions;

    public class CalendarEventStatusValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (value)
            {
                case string type when CalendarEventStatuses.IsKnownStatus(type):
                    return ValidationResult.Success;
                case string type:
                    var validTypes = string.Join(", ", CalendarEventStatuses.All);
                    return new ValidationResult($"Calendar event status `{type}` is not recognized. Must be one of the ${validTypes}");
                default:
                    return new ValidationResult("Status must be string");
            }
        }
    }
}