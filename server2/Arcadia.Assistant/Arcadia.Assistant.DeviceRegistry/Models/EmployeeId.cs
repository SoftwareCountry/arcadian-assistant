namespace Arcadia.Assistant.DeviceRegistry.Models
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    public class EmployeeId : IComparable<EmployeeId>, IEquatable<EmployeeId>
    {
        public EmployeeId()
        {
        }

        public EmployeeId(string id)
        {
            this.Id = id;
        }

        private string Id { get; } = string.Empty;

        public int CompareTo([AllowNull] EmployeeId other)
        {
            if (other == null)
            {
                return 1;
            }

            return string.Compare(this.Id, other.Id, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool Equals([AllowNull] EmployeeId other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Id.Equals(other.Id, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}