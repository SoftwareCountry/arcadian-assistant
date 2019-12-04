namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;

    public class StorageItem
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Category { get; set; } = string.Empty;

        public bool AllDayEvent { get; set; }

        public string CalendarEventId { get; set; } = string.Empty;
    }
}