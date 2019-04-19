namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;

    public class StorageItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Category { get; set; }

        public string CalendarEventId { get; set; }
    }
}