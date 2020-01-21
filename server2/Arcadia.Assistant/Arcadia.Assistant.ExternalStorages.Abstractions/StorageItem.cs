namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;
    using System.Text.Json.Serialization;

    public class StorageItem
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("Description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("StartDate")]
        public DateTime StartDate { get; set; }

        [JsonPropertyName("EndDate")]
        public DateTime EndDate { get; set; }

        [JsonPropertyName("Category")]
        public string Category { get; set; } = string.Empty;

        [JsonPropertyName("AllDayEvent")]
        public bool AllDayEvent { get; set; }

        [JsonPropertyName("CalendarEventId")]
        public string CalendarEventId { get; set; } = string.Empty;
    }
}