namespace Arcadia.Assistant.Avatars
{
    using System.Runtime.Serialization;

    [DataContract]
    public class AvatarState
    {
        [DataMember]
        public string Hash { get; set; }

        [DataMember]
        public byte[] Image { get; set; }
    }
}