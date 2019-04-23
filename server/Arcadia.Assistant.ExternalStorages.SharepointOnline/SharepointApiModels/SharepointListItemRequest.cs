namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.SharepointApiModels
{
    using System.Runtime.Serialization;

    [DataContract]
    public class SharepointListItemRequest : SharepointListItem
    {
        [DataMember(Name = "__metadata")]
        public MetadataRequest Metadata { get; set; }

        [DataContract]
        public class MetadataRequest
        {
            [DataMember(Name = "type")]
            public string Type { get; set; }
        }
    }
}