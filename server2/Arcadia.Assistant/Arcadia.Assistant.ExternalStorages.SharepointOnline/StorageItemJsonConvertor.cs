namespace Arcadia.Assistant.ExternalStorages.SharepointOnline
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;

    using Abstractions;

    using Contracts;

    public class StorageItemJsonConvertor
    {
        public StorageItemJsonConvertor(ISharepointFieldsMapper fieldsMapper)
        {
            this.FieldsMapper = fieldsMapper;
        }

        private ISharepointFieldsMapper FieldsMapper { get; }

        #region public interface

        public (
            string Id,
            string Title,
            string Description,
            string StartDate,
            string EndDate,
            string Category,
            string AllDayEvent,
            string CalendarEventId
            ) GetFieldNames()
        {
            var idField = this.FieldsMapper.GetSharepointField(si => si.Id);
            var titleField = this.FieldsMapper.GetSharepointField(si => si.Title);
            var descriptionField = this.FieldsMapper.GetSharepointField(si => si.Description);
            var startDateField = this.FieldsMapper.GetSharepointField(si => si.StartDate);
            var endDateField = this.FieldsMapper.GetSharepointField(si => si.EndDate);
            var categoryField = this.FieldsMapper.GetSharepointField(si => si.Category);
            var allDayEvent = this.FieldsMapper.GetSharepointField(si => si.AllDayEvent);
            var calendarEventIdField = this.FieldsMapper.GetSharepointField(si => si.CalendarEventId);
            return (idField, titleField, descriptionField, startDateField, endDateField, categoryField, allDayEvent, calendarEventIdField);
        }

        public string StorageItemToRequestJson(StorageItem item, string? listItemType)
        {
            var properties = new Dictionary<string, string>
            {
                [$"{this.FieldsMapper.GetSharepointField(x => x.Title)}"] = $"\"{item.Title}\"",
                [$"{this.FieldsMapper.GetSharepointField(x => x.Description)}"] = $"\"{item.Description}\"",
                [$"{this.FieldsMapper.GetSharepointField(x => x.StartDate)}"] = $"\"{item.StartDate.ToString("d")}\"",
                [$"{this.FieldsMapper.GetSharepointField(x => x.EndDate)}"] = $"\"{item.EndDate.ToString("d")}\"",
                [$"{this.FieldsMapper.GetSharepointField(x => x.Category)}"] = $"\"{item.Category}\"",
                [$"{this.FieldsMapper.GetSharepointField(x => x.AllDayEvent)}"] = item.AllDayEvent ? "true" : "false",
                [$"{this.FieldsMapper.GetSharepointField(x => x.CalendarEventId)}"] = $"\"{item.CalendarEventId}\""
            };

            if (item.Id != 0)
            {
                properties.Add($"{this.FieldsMapper.GetSharepointField(x => x.Id)}", item.Id.ToString());
            }

            if (listItemType != null)
            {
                properties.Add("__metadata", $"{{ \"type\":\"{listItemType}\" }}");
            }

            return $"{{ {string.Join(',', properties.Keys.Select(k => $"\"{k}\":{properties[k]}"))} }}";
        }

        public StorageItem? JsonToStorageItem(JsonElement item)
        {
            var allDayEvent = this.GetJsonBool(item, this.FieldsMapper.GetSharepointField(x => x.AllDayEvent));
            return new StorageItem
            {
                Id = this.GetJsonInt(item, this.FieldsMapper.GetSharepointField(x => x.Id)),
                Title = this.GetJsonString(item, this.FieldsMapper.GetSharepointField(x => x.Title)),
                Description = this.GetJsonString(item, this.FieldsMapper.GetSharepointField(x => x.Description)),
                StartDate = this.GetJsonDate(item, this.FieldsMapper.GetSharepointField(x => x.StartDate), DateTime.MinValue, allDayEvent),
                EndDate = this.GetJsonDate(item, this.FieldsMapper.GetSharepointField(x => x.EndDate), DateTime.UtcNow, allDayEvent),
                Category = this.GetJsonString(item, this.FieldsMapper.GetSharepointField(x => x.Category)),
                AllDayEvent = allDayEvent,
                CalendarEventId = this.GetJsonString(item, this.FieldsMapper.GetSharepointField(x => x.CalendarEventId))
            };
        }

        #endregion

        #region private members

        private string GetJsonString(JsonElement item, string propName, string defaultValue = "")
        {
            return item.TryGetProperty(propName, out var prop) && prop.ValueKind != JsonValueKind.Null ? prop.GetString() : defaultValue;
        }

        private int GetJsonInt(JsonElement item, string propName, int defaultValue = 0)
        {
            return item.TryGetProperty(propName, out var prop) && prop.ValueKind != JsonValueKind.Null && prop.TryGetInt32(out var val) ? val : defaultValue;
        }

        private bool GetJsonBool(JsonElement item, string propName, bool defaultValue = false)
        {
            return item.TryGetProperty(propName, out var prop) && prop.ValueKind != JsonValueKind.Null ? prop.GetBoolean() : defaultValue;
        }

        private DateTime GetJsonDate(JsonElement item, string propName, DateTime defaultValue, bool dateOnly = false)
        {
            var date = item.TryGetProperty(propName, out var prop) && prop.ValueKind != JsonValueKind.Null && prop.TryGetDateTime(out var val) ? val : defaultValue;
            ;
            return dateOnly ? date.Date : date;
        }

        #endregion
    }
}