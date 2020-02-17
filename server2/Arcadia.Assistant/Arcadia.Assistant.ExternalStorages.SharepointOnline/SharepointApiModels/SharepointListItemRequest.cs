﻿namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels
{
    using System.Text.Json.Serialization;

    using Abstractions;

    public class SharepointListItemRequest : StorageItem
    {
        public SharepointListItemRequest(StorageItem storage, string? type = null)
        {
            this.Id = storage.Id;
            this.Title = storage.Title;
            this.Description = storage.Description;
            this.StartDate = storage.StartDate;
            this.EndDate = storage.EndDate;
            this.Category = storage.Category;
            this.AllDayEvent = storage.AllDayEvent;
            this.CalendarEventId = storage.CalendarEventId;
            if (type != null)
            {
                this.Metadata = new MetadataRequest(type);
            }
        }

        [JsonPropertyName("__metadata")]
        public MetadataRequest Metadata { get; set; } = new MetadataRequest();

        public class MetadataRequest
        {
            public MetadataRequest()
            {
            }

            public MetadataRequest(string type)
            {
                this.Type = type;
            }

            [JsonPropertyName("type")]
            public string? Type { get; set; }
        }
    }
}