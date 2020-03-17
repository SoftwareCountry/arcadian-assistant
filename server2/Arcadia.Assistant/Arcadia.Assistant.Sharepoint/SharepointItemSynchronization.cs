namespace Arcadia.Assistant.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CalendarEvent;

    using Employees.Contracts;

    using ExternalStorages.Abstractions;

    using Microsoft.Extensions.Logging;

    public abstract class SharepointItemSynchronizationBase
    {
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        public static event EventHandler<SharepointSynchronizationArgs> SharepointItemSynchronizedEvent;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.

        protected void SendEvent(string calendar, EmployeeId employeeId, SharepointSynchronizationArgs.SynchronizationEventType eventType, StorageItem item)
        {
            SharepointItemSynchronizedEvent?.Invoke(this, new SharepointSynchronizationArgs
            {
                Calendar = calendar,
                EmployeeIds = new List<EmployeeId>
                    { employeeId },
                EventType = eventType,
                Item = item
            });
        }

        protected void SendEvent(string calendar, IEnumerable<EmployeeId> employeeIds, SharepointSynchronizationArgs.SynchronizationEventType eventType, StorageItem item)
        {
            SharepointItemSynchronizedEvent?.Invoke(this, new SharepointSynchronizationArgs
            {
                Calendar = calendar,
                EmployeeIds = employeeIds,
                EventType = eventType,
                Item = item
            });
        }

        public sealed class SharepointSynchronizationArgs : EventArgs
        {
            public enum SynchronizationEventType
            {
                add,
                update,
                delete
            }

            public string Calendar { get; set; }

            public IEnumerable<EmployeeId> EmployeeIds { get; set; }

            public SynchronizationEventType EventType { get; set; }

            public StorageItem Item { get; set; }
        }
    }

    public abstract class SharepointItemSynchronization<T> : SharepointItemSynchronizationBase
    {
        protected readonly IExternalStorage ExternalStorage;
        protected readonly ILogger? Logger;
        private readonly IEqualityComparer<StorageItem> sharepointStorageItemComparer = new SharepointStorageItemComparer();

        protected SharepointItemSynchronization(IExternalStorage externalStorage, ILogger? logger = null)
        {
            this.ExternalStorage = externalStorage;
            this.Logger = logger;
        }

        protected abstract string ItemEventType { get; }

        public async Task SynchronizeItems(string calendar, EmployeeMetadata[] departmentEmployees, Dictionary<string, T> values, IEnumerable<StorageItem> storageItemsList, CancellationToken cancellationToken)
        {
            try
            {
                var storageItems = storageItemsList.Where(x => x.Category == this.ItemEventType).ToList();
                var removeStorageItems = storageItems.Where(x => !values.Keys.Contains(x.CalendarEventId)).ToArray();

                // insert or update items
                foreach (var workHourEventId in values.Keys)
                {
                    var employeeMetadata = departmentEmployees.Single(x => x.EmployeeId == this.GetItemEmployeeId(values[workHourEventId]));
                    await this.UpsertItem(workHourEventId, calendar, values[workHourEventId], employeeMetadata, this.ExternalStorage, cancellationToken);
                }

                // remove redundant items
                foreach (var item in removeStorageItems)
                {
                    await this.ExternalStorage.DeleteItem(
                        calendar,
                        item.Id.ToString(),
                        cancellationToken);
                    this.SendEvent(calendar, departmentEmployees.Select(x => x.EmployeeId), SharepointSynchronizationArgs.SynchronizationEventType.delete, item);
                }
            }
            catch (Exception e)
            {
                this.Logger?.LogError(e, $"'{typeof(T)}' synchronization error");
            }
        }

        protected abstract DatesPeriod GetItemDatePeriod(T item);

        protected abstract EmployeeId GetItemEmployeeId(T item);

        private async Task UpsertItem(string eventId, string calendar, T item, EmployeeMetadata employeeMetadata, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var datesPeriod = this.GetItemDatePeriod(item);
            var upsertItem = this.CalendarEventToStorageItem(eventId, this.ItemEventType, datesPeriod, employeeMetadata);
            await this.UpsertStorageItem(eventId, calendar, employeeMetadata.EmployeeId, datesPeriod, upsertItem, externalStorage, cancellationToken);
        }

        private async Task<StorageItem> GetSharepointItemForCalendarEvent(IExternalStorage externalStorage, string calendar, string eventId)
        {
            var existingItems = await externalStorage.GetItems(
                calendar,
                new[] { new EqualCondition(x => x.CalendarEventId, eventId) });
            return existingItems.SingleOrDefault();
        }

        private async Task UpsertStorageItem(string eventId, string calendar, EmployeeId employeeId, DatesPeriod datesPeriod, StorageItem upsertItem, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var storageItem = await this.GetSharepointItemForCalendarEvent(externalStorage, calendar, eventId);

            if (storageItem == null)
            {
                await externalStorage.AddItem(
                    calendar,
                    upsertItem,
                    cancellationToken);
                this.SendEvent(calendar, employeeId, SharepointSynchronizationArgs.SynchronizationEventType.add, upsertItem);
            }
            else
            {
                upsertItem.Id = storageItem.Id;
                if (!this.sharepointStorageItemComparer.Equals(upsertItem, storageItem))
                {
                    await externalStorage.UpdateItem(
                        calendar,
                        upsertItem,
                        cancellationToken);
                    this.SendEvent(calendar, employeeId, SharepointSynchronizationArgs.SynchronizationEventType.update, upsertItem);
                }
            }
        }

        private StorageItem CalendarEventToStorageItem(string eventId, string calendarEventType, DatesPeriod period, EmployeeMetadata employeeMetadata)
        {
            var totalHours = period.FinishWorkingHour - period.StartWorkingHour;
            var longEvent = calendarEventType == CalendarEventTypes.Vacation || calendarEventType == CalendarEventTypes.Sickleave;
            var title = longEvent
                ? $"{employeeMetadata.Name} ({calendarEventType})"
                : $"{employeeMetadata.Name} ({calendarEventType}: {totalHours} hours)";

            var storageItem = new StorageItem
            {
                Title = title,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Category = calendarEventType,
                AllDayEvent = longEvent,
                CalendarEventId = eventId
            };

            return storageItem;
        }
    }
}