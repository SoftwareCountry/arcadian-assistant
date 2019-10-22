namespace Arcadia.Assistant.Avatars.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Photo
    {
        [DataMember]
        public string? MimeType { get; set; }

        [DataMember]
        public int Width { get; set; }

        [DataMember]
        public int Height { get; set; }

        [DataMember]
        public byte[]? Bytes { get; set; } //TODO: switch to ReadOnlySpan
    }
}