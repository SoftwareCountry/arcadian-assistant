namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Collections.Generic;

    public class Message
    {
        public string MessageId { get; }

        public string PostedByEmployeeId { get; }

        public string Title { get; }

        public string Text { get; }

        public DateTime DatePosted { get; }

        public Message(string messageId, string postedByEmployeeId, string title, string text, DateTime datePosted)
        {
            this.Title = title;
            this.Text = text;
            this.DatePosted = datePosted;
            this.MessageId = messageId;
            this.PostedByEmployeeId = postedByEmployeeId;
        }

        private sealed class MessageIdEqualityComparer : IEqualityComparer<Message>
        {
            public bool Equals(Message x, Message y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.MessageId.Equals(y.MessageId);
            }

            public int GetHashCode(Message obj)
            {
                return obj.MessageId.GetHashCode();
            }
        }

        public static IEqualityComparer<Message> MessageIdComparer { get; } = new MessageIdEqualityComparer();
    }
}