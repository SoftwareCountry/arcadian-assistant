namespace Arcadia.Assistant.Notifications.Push
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class PushNotificationTarget
    {
        [DataMember]
        public const string Type = "devices_target";

        [DataMember]
        public IEnumerable<string> Devices { get; set; }
    }
}