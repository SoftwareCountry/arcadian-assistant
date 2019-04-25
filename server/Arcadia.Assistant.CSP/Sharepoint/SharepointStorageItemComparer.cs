namespace Arcadia.Assistant.CSP.Sharepoint
{
    using System.Collections.Generic;

    using Arcadia.Assistant.ExternalStorages.Abstractions;

    public sealed class SharepointStorageItemComparer : IEqualityComparer<StorageItem>
    {
        public bool Equals(StorageItem x, StorageItem y)
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

            return
                string.Equals(x.Id, y.Id) &&
                string.Equals(x.Title, y.Title) &&
                string.Equals(x.Description, y.Description) &&
                x.StartDate.Equals(y.StartDate) &&
                x.EndDate.Equals(y.EndDate) &&
                string.Equals(x.Category, y.Category) &&
                x.AllDayEvent == y.AllDayEvent &&
                string.Equals(x.CalendarEventId, y.CalendarEventId);
        }

        public int GetHashCode(StorageItem obj)
        {
            unchecked
            {
                var hashCode = obj.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Title.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Description.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.StartDate.Date.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.EndDate.Date.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Category.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.AllDayEvent.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.CalendarEventId.GetHashCode();
                return hashCode;
            }
        }
    }
}