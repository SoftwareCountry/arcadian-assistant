namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System.Collections.Generic;

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
                x.Id.Equals(y.Id) &&
                x.Title.Equals(y.Title) &&
                x.Description.Equals(y.Description) &&
                x.StartDate.Date.Equals(y.StartDate.Date) &&
                x.EndDate.Date.Equals(y.EndDate.Date) &&
                x.Category.Equals(y.Category) &&
                x.AllDayEvent == y.AllDayEvent &&
                x.CalendarEventId.Equals(y.CalendarEventId);
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