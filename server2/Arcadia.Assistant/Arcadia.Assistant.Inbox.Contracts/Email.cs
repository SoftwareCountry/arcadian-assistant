namespace Arcadia.Assistant.Inbox.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class Email
    {
        public Email()
        {
        }

        public Email(
            uint uniqueId,
            DateTimeOffset date,
            string sender,
            string subject,
            string text,
            byte[][] attachments)
        {
            this.UniqueId = uniqueId;
            this.Date = date;
            this.Sender = sender;
            this.Subject = subject;
            this.Text = text;
            this.Attachments = attachments;
        }

        [DataMember]
        public uint UniqueId { get; private set; }

        [DataMember]
        public DateTimeOffset Date { get; private set; }

        [DataMember]
        public string Sender { get; private set; } = string.Empty;

        [DataMember]
        public string Subject { get; private set; } = string.Empty;

        [DataMember]
        public string Text { get; private set; } = string.Empty;

        [DataMember]
        public byte[][] Attachments { get; private set; } = new byte[0][];
    }
}