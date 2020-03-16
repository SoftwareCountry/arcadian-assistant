namespace Arcadia.Assistant.Permissions.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public struct UserIdentity
    {
        [DataMember] public readonly string Value;

        public UserIdentity(string value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{nameof(this.Value)}: {this.Value}";
        }

        public bool Equals(UserIdentity other)
        {
            return this.Value == other.Value;
        }

        public override bool Equals(object? obj)
        {
            return obj is UserIdentity other && Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}