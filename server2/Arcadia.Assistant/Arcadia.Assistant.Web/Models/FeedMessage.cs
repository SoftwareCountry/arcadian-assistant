using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Arcadia.Assistant.Web.Models
{
    using UserFeeds.Contracts;

    [DataContract]
    public class FeedMessage
    {
        [DataMember]
        public readonly string MessageId;

        [DataMember]
        public readonly string EmployeeId;

        [DataMember]
        public readonly string Title;

        [DataMember]
        public readonly string Text;

        [DataMember]
        public readonly DateTime DatePosted;

        public FeedMessage(string employeeId, FeedItem feedItem)
        {
            this.MessageId = feedItem.Id;
            this.EmployeeId = employeeId;
            this.Title = feedItem.Text;
            this.Text = feedItem.Description;
            this.DatePosted = feedItem.Date;
        }

        public FeedMessage(string messageId, string employeeId, string title, string text, DateTime datePosted)
        {
            this.MessageId = messageId;
            this.EmployeeId = employeeId;
            this.Title = title;
            this.Text = text;
            this.DatePosted = datePosted;
        }

        private sealed class MessageIdEqualityComparer : IEqualityComparer<FeedMessage>
        {
            public bool Equals(FeedMessage x, FeedMessage y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                return x.MessageId.Equals(y.MessageId);
            }

            public int GetHashCode(FeedMessage obj)
            {
                return obj.MessageId.GetHashCode();
            }
        }

        public static IEqualityComparer<FeedMessage> MessageIdComparer { get; } = new MessageIdEqualityComparer();
    }
}
