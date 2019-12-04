namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SharepointListItemRequest
    {
        [DataMember(Name = "__metadata")]
        public MetadataRequest Metadata { get; set; } = new MetadataRequest();

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; } = string.Empty;

        [DataMember]
        public string Description { get; set; } = string.Empty;

        [DataMember]
        public string EventDate { get; set; } = string.Empty;

        [DataMember]
        public string EndDate { get; set; } = string.Empty;

        [DataMember]
        public string Category { get; set; } = string.Empty;

        [DataMember(Name = "fAllDayEvent")]
        public bool AllDayEvent { get; set; }

        [DataMember]
        public string CalendarEventId { get; set; } = string.Empty;

        [DataContract]
        public class MetadataRequest
        {
            [DataMember(Name = "type")]
            public string? Type { get; set; }
        }
    }
}