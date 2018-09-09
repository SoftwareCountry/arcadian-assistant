namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;

    public class RegistryRecordKey
    {
        public string Name { get; }

        public DateTime? Birthday { get; }

        public RegistryRecordKey(string name, DateTime birthday)
        {
            this.Name = name;
            this.Birthday = birthday;
        }

        private sealed class NameBirthdayEqualityComparer : IEqualityComparer<RegistryRecordKey>
        {
            public bool Equals(RegistryRecordKey x, RegistryRecordKey y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }

                if (ReferenceEquals(x, null))
                {
                    return false;
                }

                if (ReferenceEquals(y, null))
                {
                    return false;
                }

                if (x.GetType() != y.GetType())
                {
                    return false;
                }

                return string.Equals(x.Name, y.Name) && x.Birthday.Equals(y.Birthday);
            }

            public int GetHashCode(RegistryRecordKey obj)
            {
                unchecked
                {
                    return ((obj.Name != null ? obj.Name.GetHashCode() : 0) * 397) ^ obj.Birthday.GetHashCode();
                }
            }
        }

        public static IEqualityComparer<RegistryRecordKey> NameBirthdayComparer { get; } = new NameBirthdayEqualityComparer();
    }
}