namespace Arcadia.Assistant.InboxEmail.Abstractions
{
    using System.Collections.Generic;
    using System.IO;

    public class Email
    {
        public Email(uint uniqueId, string text, IEnumerable<Stream> attachments)
        {
            this.UniqueId = uniqueId;
            this.Text = text;
            this.Attachments = attachments;
        }

        public uint UniqueId { get; }

        public string Text { get; }

        public IEnumerable<Stream> Attachments { get; }
    }
}