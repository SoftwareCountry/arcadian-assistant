namespace Arcadia.Assistant.Employees.Contracts
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public struct DepartmentId : IComparable<DepartmentId>
    {
        [DataMember]
        public int Value { get; private set; }

        public DepartmentId(int value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public bool Equals(DepartmentId other)
        {
            return this.Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            return obj is DepartmentId other && this.Equals(other);
        }

        public override int GetHashCode()
        {
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return this.Value;
        }

        public static bool operator ==(DepartmentId id1, DepartmentId id2)
        {
            return id1.Equals(id2);
        }

        public static bool operator !=(DepartmentId id1, DepartmentId id2)
        {
            return !(id1 == id2);
        }

        public int CompareTo(DepartmentId other)
        {
            return this.Value.CompareTo(other.Value);
        }
    }
}