namespace Arcadia.Assistant.InboxEmail.Abstractions
{
    using System;
    using System.Collections.Generic;

    public class Email
    {
        public Email(
            uint uniqueId,
            DateTimeOffset date,
            string sender,
            string subject,
            string text,
            IEnumerable<byte[]> attachments)
        {
            this.UniqueId = uniqueId;
            this.Date = date;
            this.Sender = sender;
            this.Subject = subject;
            this.Text = text;
            this.Attachments = attachments;
        }

        public uint UniqueId { get; }

        public DateTimeOffset Date { get; }

        public string Sender { get; }

        public string Subject { get; }

        public string Text { get; }

        public IEnumerable<byte[]> Attachments { get; }
    }
}