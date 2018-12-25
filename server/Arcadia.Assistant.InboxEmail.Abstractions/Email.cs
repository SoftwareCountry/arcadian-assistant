namespace Arcadia.Assistant.InboxEmail.Abstractions
{
    using System.Collections.Generic;

    public class Email
    {
        public Email(uint uniqueId, string text, IEnumerable<byte[]> attachments)
        {
            this.UniqueId = uniqueId;
            this.Text = text;
            this.Attachments = attachments;
        }

        public uint UniqueId { get; }

        public string Text { get; }

        public IEnumerable<byte[]> Attachments { get; }
    }
}