namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.ComponentModel.DataAnnotations;

    public class WorkHoursChangeValidator : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            switch (validationContext.ObjectInstance)
            {
                case CalendarEventModel model when model.Type != CalendarEventTypes.Dayoff && model.Type != CalendarEventTypes.Workout:
                    return ValidationResult.Success;
                case CalendarEventModel model when model.Dates.StartDate != model.Dates.EndDate:
                    return new ValidationResult("Model dates must match");
                case CalendarEventModel _:
                    return ValidationResult.Success;
                default:
                    return new ValidationResult($"Attribute usage error: ValidationContext must be applied to {typeof(CalendarEventModel)}");
            }
        }
    }
}