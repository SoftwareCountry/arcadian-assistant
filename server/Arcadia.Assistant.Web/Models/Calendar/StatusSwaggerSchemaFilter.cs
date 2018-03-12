namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;

    using Arcadia.Assistant.Calendar.Abstractions;

    using Swashbuckle.AspNetCore.Swagger;
    using Swashbuckle.AspNetCore.SwaggerGen;

    public class StatusSwaggerSchemaFilter : ISchemaFilter
    {
        public void Apply(Schema model, SchemaFilterContext context)
        {
            model.Properties[nameof(CalendarEventsModel.Type).ToLower()].Enum = new List<object>(CalendarEventTypes.All);
            model.Properties[nameof(CalendarEventsModel.Status).ToLower()].Enum = new List<object>(CalendarEventStatuses.All);
        }
    }
}