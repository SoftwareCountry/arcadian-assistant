namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SharepointListItemRequest
    {
        [DataMember(Name = "__metadata")]
        public MetadataRequest Metadata { get; set; }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string EventDate { get; set; }

        [DataMember]
        public string EndDate { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember(Name = "fAllDayEvent")]
        public bool AllDayEvent { get; set; }

        [DataMember]
        public string CalendarEventId { get; set; }

        [DataContract]
        public class MetadataRequest
        {
            [DataMember(Name = "type")]
            public string Type { get; set; }
        }
    }
}