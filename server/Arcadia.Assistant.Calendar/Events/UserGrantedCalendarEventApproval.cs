// ReSharper disable once CheckNamespace
// Needed for Akka Event Journal
namespace Arcadia.Assistant.Calendar
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class UserGrantedCalendarEventApproval
    {
        [DataMember]
        public string EventId { get; set; }

        [DataMember]
        public DateTimeOffset TimeStamp { get; set; }

        [DataMember]
        public string UserId { get; set; }
    }
}