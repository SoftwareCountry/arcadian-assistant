namespace Arcadia.Assistant.CSP.SickLeaves
{
    using System.Collections.Generic;
    using System.Linq;

    using Arcadia.Assistant.Calendar.Abstractions;

    internal class DatabaseSickLeavesCache
    {
        private readonly Dictionary<string, CalendarEventWithAdditionalData> cache;

        public DatabaseSickLeavesCache(Dictionary<string, CalendarEventWithAdditionalData> values)
        {
            this.cache = new Dictionary<string, CalendarEventWithAdditionalData>(values);
        }

        public CalendarEventWithAdditionalData this[string index]
        {
            get => this.cache.TryGetValue(index, out var value) ? value : null;
            set => this.cache[index] = value;
        }

        public Diff Difference(Dictionary<string, CalendarEventWithAdditionalData> values)
        {
            var createdEvents = new List<CalendarEventWithAdditionalData>();
            var updatedEvents = new List<CalendarEventWithAdditionalData>();

            foreach (var @event in values.Values)
            {
                if (!this.cache.TryGetValue(@event.CalendarEvent.EventId, out var cacheEvent))
                {
                    if (@event.CalendarEvent.Status == VacationStatuses.Requested)
                    {
                        createdEvents.Add(@event);
                    }
                    else
                    {
                        updatedEvents.Add(@event);
                    }
                }
                else if (cacheEvent.CalendarEvent.Status != @event.CalendarEvent.Status || cacheEvent.CalendarEvent.Dates != @event.CalendarEvent.Dates)
                {
                    updatedEvents.Add(@event);
                }
            }

            var removedEvents = this.cache
                .Where(x => !values.ContainsKey(x.Key))
                .Select(x => x.Value)
                .ToList();

            return new Diff(createdEvents, updatedEvents, removedEvents);
        }

        public void Update(Diff difference)
        {
            var updatedEvents = difference.Created
                .Union(difference.Updated);

            foreach (var @event in updatedEvents)
            {
                this.cache[@event.CalendarEvent.EventId] = @event;
            }

            foreach (var @event in difference.Removed)
            {
                if (this.cache.ContainsKey(@event.CalendarEvent.EventId))
                {
                    this.cache.Remove(@event.CalendarEvent.EventId);
                }
            }
        }

        public class Diff
        {
            public Diff(
                IEnumerable<CalendarEventWithAdditionalData> created,
                IEnumerable<CalendarEventWithAdditionalData> updated,
                IEnumerable<CalendarEventWithAdditionalData> removed)
            {
                this.Created = created;
                this.Updated = updated;
                this.Removed = removed;
            }

            public IEnumerable<CalendarEventWithAdditionalData> Created { get; }

            public IEnumerable<CalendarEventWithAdditionalData> Updated { get; }

            public IEnumerable<CalendarEventWithAdditionalData> Removed { get; }
        }
    }
}