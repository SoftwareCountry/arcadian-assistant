namespace Arcadia.Assistant.DeviceRegistry.Contracts.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct DeviceId : IComparable<DeviceId>, IEquatable<DeviceId>
    {
        [DataMember]
        public string Value { get; private set; }

        public DeviceId(string value)
        {
            this.Value = value;
        }

        public int CompareTo(DeviceId other)
        {
            return string.Compare(this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Equals(DeviceId other)
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

            return obj is DeviceId other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Value.GetHashCode();
        }

        public static bool operator ==(DeviceId id1, DeviceId id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(DeviceId id1, DeviceId id2)
        {
            return !(id1 == id2);
        }
    }
}