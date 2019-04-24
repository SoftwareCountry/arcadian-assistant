namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class SharepointListItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public DateTime EventDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember(Name = "fAllDayEvent")]
        public bool AllDayEvent { get; set; }

        [DataMember]
        public string CalendarEventId { get; set; }
    }
}