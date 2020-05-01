namespace Arcadia.Assistant.Employees.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct EmployeeId : IComparable<EmployeeId>, IEquatable<EmployeeId>
    {
        [DataMember]
        public int Value { get; private set; }

        public EmployeeId(int value)
        {
            this.Value = value;
        }

        public int CompareTo(EmployeeId other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public bool Equals(EmployeeId other)
        {
            return this.Value == other.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is EmployeeId other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Value;
        }

        public static bool operator ==(EmployeeId id1, EmployeeId id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(EmployeeId id1, EmployeeId id2)
        {
            return !(id1 == id2);
        }
    }
}