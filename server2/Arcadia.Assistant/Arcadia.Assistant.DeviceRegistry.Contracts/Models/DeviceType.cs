namespace Arcadia.Assistant.DeviceRegistry.Contracts.Models
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct DeviceType : IEquatable<DeviceType>
    {
        [DataMember]
        public string Value { get; private set; }

        public DeviceType(string value)
        {
            this.Value = value;
        }

        public bool Equals(DeviceType other)
        {
            return string.Equals(this.Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool Equals(string otherValue)
        {
            return string.Equals(this.Value, otherValue, StringComparison.InvariantCultureIgnoreCase);
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

            return obj is DeviceType other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Value.GetHashCode();
        }

        public static bool operator ==(DeviceType id1, DeviceType id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator ==(DeviceType id1, string id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator ==(string id1, DeviceType id2)
        {
            return id2.Equals(id1);
        }

        public static bool operator !=(DeviceType id1, DeviceType id2)
        {
            return !(id1 == id2);
        }

        public static bool operator !=(DeviceType id1, string id2)
        {
            return !(id1 == id2);
        }

        public static bool operator !=(string id1, DeviceType id2)
        {
            return !(id1 == id2);
        }
    }
}