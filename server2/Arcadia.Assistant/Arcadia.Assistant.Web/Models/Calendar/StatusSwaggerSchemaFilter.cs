namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NJsonSchema.Generation;

    public class StatusSwaggerSchemaFilter : ISchemaProcessor
    {
        //public void Apply(Schema model, SchemaFilterContext context)
        //{
        //    model.Properties[nameof(CalendarEventsModel.Type).ToLower()].Enum = new List<object>(CalendarEventTypes.All);

        //    var statuses = new CalendarEventStatuses();
        //    var possibleStatusValues = CalendarEventTypes.All.SelectMany(statuses.AllForType).Distinct().ToList<object>();
        //    model.Properties[nameof(CalendarEventsModel.Status).ToLower()].Enum = possibleStatusValues;
        //}

        public void Process(SchemaProcessorContext context)
        {
            if (!context.Schema.Properties.ContainsKey(nameof(CalendarEventsModel.Type).ToLower()))
            {
                return;
            }

            context.Schema.Properties[nameof(CalendarEventsModel.Type).ToLower()].EnumerationNames = new Collection<string>(CalendarEventTypes.All);

            var statuses = new CalendarEventStatuses();
            var possibleStatusValues = CalendarEventTypes.All.SelectMany(statuses.AllForType).Distinct().ToList();
            context.Schema.Properties[nameof(CalendarEventsModel.Status).ToLower()].EnumerationNames = new Collection<string>(possibleStatusValues);
        }
    }
}