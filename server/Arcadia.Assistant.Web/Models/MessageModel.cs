namespace Arcadia.Assistant.Web.Models
{
    using System;
    using System.Runtime.Serialization;

    using Arcadia.Assistant.Feeds;

    [DataContract]
    public class MessageModel
    {
        [DataMember]
        public string MessageId { get; }

        [DataMember]
        public string EmployeeId { get; }

        [DataMember]
        public string Title { get; }

        [DataMember]
        public string Text { get; }

        [DataMember]
        public DateTime DatePosted { get; }

        public MessageModel(Message message)
        {
            this.MessageId = message.MessageId;
            this.EmployeeId = message.PostedByEmployeeId;
            this.Title = message.Title;
            this.Text = message.Text;
            this.DatePosted = message.DatePosted;
        }
    }
}