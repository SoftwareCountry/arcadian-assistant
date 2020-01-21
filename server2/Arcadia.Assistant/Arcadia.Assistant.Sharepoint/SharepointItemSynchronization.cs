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

    public abstract class SharepointItemSynchronization<T>
    {
        #region ctor

        protected SharepointItemSynchronization(IExternalStorage externalStorage, ILogger? logger = null)
        {
            this.ExternalStorage = externalStorage;
            this.Logger = logger;
        }

        #endregion

        #region public interface

        public async Task SynchronizeItem(string calendar, EmployeeMetadata[] departmentEmployes, Dictionary<string, T> values, IEnumerable<StorageItem> storageItemsList, CancellationToken cancellationToken)
        {
            try
            {
                var storageItems = storageItemsList.Where(x => x.Category == this.ItemEventType).ToList();
                var removeStorageItems = storageItems.Where(x => !values.Keys.Contains(x.CalendarEventId)).ToArray();

                // insert or update items
                foreach (var workHourEventId in values.Keys)
                {
                    var employeeMetadata = departmentEmployes.Single(x => x.EmployeeId == this.GetItemEmployeeId(values[workHourEventId]));
                    await this.UpsertItem(workHourEventId, calendar, values[workHourEventId], employeeMetadata, this.ExternalStorage, cancellationToken);
                }

                // remove redundant items
                foreach (var item in removeStorageItems)
                {
                    await this.ExternalStorage.DeleteItem(
                        calendar,
                        item.Id.ToString());
                }
            }
            catch (Exception e)
            {
                this.Logger?.LogError(e, $"'{typeof(T)}' synchronization error");
            }
        }

        #endregion

        #region variables

        private readonly IEqualityComparer<StorageItem> sharepointStorageItemComparer = new SharepointStorageItemComparer();
        protected readonly IExternalStorage ExternalStorage;
        protected readonly ILogger? Logger;

        #endregion

        #region virtual interface

        protected abstract string ItemEventType { get; }

        protected abstract DatesPeriod GetItemDatePeriod(T item);

        protected abstract EmployeeId GetItemEmployeeId(T item);

        #endregion

        #region private

        private async Task UpsertItem(string eventId, string calendar, T item, EmployeeMetadata employeeMetadata, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var datesPeriod = this.GetItemDatePeriod(item);
            var upsertItem = this.CalendarEventToStorageItem(eventId, this.ItemEventType, datesPeriod, employeeMetadata);
            await this.UpsertStorageItem(eventId, calendar, datesPeriod, upsertItem, externalStorage, cancellationToken);
        }

        private async Task<StorageItem> GetSharepointItemForCalendarEvent(IExternalStorage externalStorage, string calendar, string eventId)
        {
            var existingItems = await externalStorage.GetItems(
                calendar,
                new[] { new EqualCondition(x => x.CalendarEventId, eventId) });
            return existingItems.SingleOrDefault();
        }

        private async Task UpsertStorageItem(string eventId, string calendar, DatesPeriod datesPeriod, StorageItem upsertItem, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var storageItem = await this.GetSharepointItemForCalendarEvent(externalStorage, calendar, eventId);

            if (storageItem == null)
            {
                await externalStorage.AddItem(
                    calendar,
                    upsertItem,
                    cancellationToken);
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

        #endregion
    }
}