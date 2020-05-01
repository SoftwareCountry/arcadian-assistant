namespace Arcadia.Assistant.UserFeeds.Contracts.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct FeedId : IComparable<FeedId>, IEquatable<FeedId>
    {
        [DataMember]
        public string Value { get; private set; }

        public FeedId(string value)
        {
            this.Value = value;
        }

        public int CompareTo(FeedId other)
        {
            return string.Compare(this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Equals(FeedId other)
        {
            return string.Equals(this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public override string ToString()
        {
            return this.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is FeedId other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Value.GetHashCode();
        }

        public static bool operator ==(FeedId id1, FeedId id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(FeedId id1, FeedId id2)
        {
            return !(id1 == id2);
        }
    }
}