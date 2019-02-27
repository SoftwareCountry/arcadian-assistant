namespace Arcadia.Assistant.CSP.Vacations
{
    using System.Collections.Generic;
    using System.Linq;
    using Arcadia.Assistant.Calendar.Abstractions;

    internal class DatabaseVacationsCache
    {
        private readonly Dictionary<string, CalendarEventWithApprovals> cache;

        public DatabaseVacationsCache(Dictionary<string, CalendarEventWithApprovals> values)
        {
            this.cache = new Dictionary<string, CalendarEventWithApprovals>(values);
        }

        public CalendarEventWithApprovals this[string index]
        {
            get => this.cache[index];
            set => this.cache[index] = value;
        }

        public Diff Difference(Dictionary<string, CalendarEventWithApprovals> values)
        {
            var createdEvents = new List<CalendarEventWithApprovals>();
            var updatedEvents = new List<CalendarEventWithApprovals>();
            var approvalsUpdatedEvents = new List<CalendarEventWithApprovals>();

            foreach (var @event in values.Values)
            {
                if (!this.cache.ContainsKey(@event.CalendarEvent.EventId))
                {
                    if (@event.CalendarEvent.Status == VacationStatuses.Requested)
                    {
                        if (!@event.Approvals.Any())
                        {
                            createdEvents.Add(@event);
                        }
                        else
                        {
                            approvalsUpdatedEvents.Add(@event);
                        }
                    }
                    else
                    {
                        updatedEvents.Add(@event);
                    }
                }
                else
                {
                    var cacheEvent = this.cache[@event.CalendarEvent.EventId];

                    if (cacheEvent.CalendarEvent.Status != @event.CalendarEvent.Status || cacheEvent.CalendarEvent.Dates != @event.CalendarEvent.Dates)
                    {
                        updatedEvents.Add(@event);
                    }

                    if (@event.CalendarEvent.Status == VacationStatuses.Requested)
                    {
                        var cacheApprovals = cacheEvent.Approvals.Select(x => x.ApprovedBy);
                        var databaseApprovals = @event.Approvals.Select(x => x.ApprovedBy).ToList();

                        if (cacheApprovals.Intersect(databaseApprovals).Count() != databaseApprovals.Count)
                        {
                            approvalsUpdatedEvents.Add(@event);
                        }
                    }
                }
            }

            var removedEvents = this.cache
                .Where(x => !values.ContainsKey(x.Key))
                .Select(x => x.Value)
                .ToList();

            return new Diff(createdEvents, updatedEvents, approvalsUpdatedEvents, removedEvents);
        }

        public void Update(Diff difference)
        {
            var updatedEvents = difference.Created
                .Union(difference.Updated)
                .Union(difference.ApprovalsUpdated);

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
                IEnumerable<CalendarEventWithApprovals> created,
                IEnumerable<CalendarEventWithApprovals> updated,
                IEnumerable<CalendarEventWithApprovals> approvalsUpdated,
                IEnumerable<CalendarEventWithApprovals> removed)
            {
                this.Created = created;
                this.Updated = updated;
                this.ApprovalsUpdated = approvalsUpdated;
                this.Removed = removed;
            }

            public IEnumerable<CalendarEventWithApprovals> Created { get; }

            public IEnumerable<CalendarEventWithApprovals> Updated { get; }

            public IEnumerable<CalendarEventWithApprovals> ApprovalsUpdated { get; }

            public IEnumerable<CalendarEventWithApprovals> Removed { get; }
        }
    }
}