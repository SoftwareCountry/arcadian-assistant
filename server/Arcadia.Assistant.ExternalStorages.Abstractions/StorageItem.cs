namespace Arcadia.Assistant.ExternalStorages.Abstractions
{
    using System;

    public class StorageItem
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public string Category { get; set; }

        public string CalendarEventId { get; set; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((StorageItem)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = this.Id.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Title.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Description.GetHashCode();
                hashCode = (hashCode * 397) ^ this.StartDate.GetHashCode();
                hashCode = (hashCode * 397) ^ this.EndDate.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Category.GetHashCode();
                hashCode = (hashCode * 397) ^ this.CalendarEventId.GetHashCode();
                return hashCode;
            }
        }

        protected bool Equals(StorageItem other)
        {
            return
                string.Equals(this.Id, other.Id) &&
                string.Equals(this.Title, other.Title) &&
                string.Equals(this.Description, other.Description) &&
                this.StartDate.Equals(other.StartDate) &&
                this.EndDate.Equals(other.EndDate) &&
                string.Equals(this.Category, other.Category) &&
                string.Equals(this.CalendarEventId, other.CalendarEventId);
        }
    }
}