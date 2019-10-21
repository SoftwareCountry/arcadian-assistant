namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NJsonSchema.Generation;

    public class CalendarEventSchemaProcessor : ISchemaProcessor
    {
        //public void Apply(Schema model, SchemaFilterContext context)
        //{
        //    model.Properties[nameof(CalendarEventModel.Type).ToLower()].Enum = new List<object>(CalendarEventTypes.All);

        //    var statuses = new CalendarEventStatuses();
        //    var possibleStatusValues = CalendarEventTypes.All.SelectMany(statuses.AllForType).Distinct().ToList<object>();
        //    model.Properties[nameof(CalendarEventModel.Status).ToLower()].Enum = possibleStatusValues;
        //}

        public void Process(SchemaProcessorContext context)
        {
            if (!context.Schema.Properties.ContainsKey(nameof(CalendarEventModel.Type).ToLower()))
            {
                return;
            }

            var typeProperty = context.Schema.Properties[nameof(CalendarEventModel.Type).ToLower()];
            typeProperty.IsRequired = true;
            typeProperty.Example = CalendarEventTypes.Vacation;
            //typeProperty.EnumerationNames = new Collection<string>(CalendarEventTypes.All);

            var statuses = new CalendarEventStatuses();
            var possibleStatusValues = CalendarEventTypes.All.SelectMany(statuses.AllForType).Distinct().ToList();
            var statusesProperties = context.Schema.Properties[nameof(CalendarEventModel.Status).ToLower()];
            statusesProperties.IsRequired = true;
            statusesProperties.Example = possibleStatusValues[0];
            statusesProperties.EnumerationNames = new Collection<string>(possibleStatusValues);
        }
    }
}