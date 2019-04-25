namespace Arcadia.Assistant.CSP.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions;

    public class SharepointStorageActor : UntypedActor, ILogReceive
    {
        private readonly Func<IExternalStorage> externalStorageProvider;
        private readonly ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings;
        private readonly IEqualityComparer<StorageItem> sharepointStorageItemComparer = new SharepointStorageItemComparer();

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SharepointStorageActor(Func<IExternalStorage> externalStorageProvider, ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings)
        {
            this.externalStorageProvider = externalStorageProvider;
            this.departmentsCalendarsSettings = departmentsCalendarsSettings;
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<SharepointStorageActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StoreCalendarEventToSharepoint msg:
                    this.OnReceiveEventUpdate(msg.Event, msg.EmployeeMetadata);
                    break;

                case RemoveCalendarEventFromSharepoint msg:
                    this.OnReceiveEventRemove(msg.Event, msg.EmployeeMetadata);
                    break;

                case CalendarEventUpsertSuccess msg:
                    this.logger.Debug($"Calendar event with id {msg.Event.EventId} was successfully upserted to Sharepoint");
                    break;

                case CalendarEventUpsertFailed msg:
                    this.logger.Error(msg.Exception, $"Error occurred when trying to upsert calendar event with id {msg.Event.EventId} to Sharepoint");
                    break;

                case CalendarEventRemoveSuccess msg:
                    this.logger.Debug($"Calendar event with id {msg.Event.EventId} was successfully removed from Sharepoint");
                    break;

                case CalendarEventRemoveFailed msg:
                    this.logger.Error(msg.Exception, $"Error occurred when trying to remove calendar event with id {msg.Event.EventId} from Sharepoint");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnReceiveEventUpdate(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            if (this.NeedToStoreCalendarEvent(@event))
            {
                this.UpsertCalendarEvent(@event, employeeMetadata)
                    .PipeTo(
                        this.Self,
                        success: () => new CalendarEventUpsertSuccess(@event),
                        failure: err => new CalendarEventUpsertFailed(@event, err));
            }
            else
            {
                this.RemoveCalendarEvent(@event, employeeMetadata)
                    .PipeTo(
                        this.Self,
                        success: () => new CalendarEventRemoveSuccess(@event),
                        failure: err => new CalendarEventRemoveFailed(@event, err));
            }
        }

        private void OnReceiveEventRemove(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            this.RemoveCalendarEvent(@event, employeeMetadata)
                .PipeTo(
                    this.Self,
                    success: () => new CalendarEventRemoveSuccess(@event),
                    failure: err => new CalendarEventRemoveFailed(@event, err));
        }

        private async Task UpsertCalendarEvent(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            var externalStorage = this.externalStorageProvider();
            var departmentCalendars = this.GetSharepointCalendarsByDepartment(employeeMetadata.DepartmentId);

            var sharepointTasks = departmentCalendars.Select(async calendar =>
            {
                var existingItem = await this.GetSharepointItemForCalendarEvent(externalStorage, calendar, @event);

                var upsertItem = this.CalendarEventToStorageItem(@event, employeeMetadata);
                upsertItem.Id = existingItem?.Id;

                if (existingItem == null)
                {
                    await externalStorage.AddItem(
                        calendar,
                        upsertItem);
                }
                else if (!this.sharepointStorageItemComparer.Equals(upsertItem, existingItem))
                {
                    await externalStorage.UpdateItem(
                        calendar,
                        upsertItem);
                }
            });

            await Task.WhenAll(sharepointTasks);
        }

        private async Task RemoveCalendarEvent(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            var externalStorage = this.externalStorageProvider();
            var departmentCalendars = this.GetSharepointCalendarsByDepartment(employeeMetadata.DepartmentId);

            var sharepointTasks = departmentCalendars.Select(async calendar =>
            {
                var existingItem = await this.GetSharepointItemForCalendarEvent(externalStorage, calendar, @event);

                if (existingItem != null)
                {
                    await externalStorage.DeleteItem(
                        calendar,
                        existingItem.Id);
                }
            });

            await Task.WhenAll(sharepointTasks);
        }

        private async Task<StorageItem> GetSharepointItemForCalendarEvent(IExternalStorage externalStorage, string calendar, CalendarEvent @event)
        {
            var existingItems = await externalStorage.GetItems(
                calendar,
                new[] { new EqualCondition(x => x.CalendarEventId, @event.EventId) });
            return existingItems.SingleOrDefault();
        }

        private StorageItem CalendarEventToStorageItem(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            var totalHours = @event.Dates.FinishWorkingHour - @event.Dates.StartWorkingHour;

            var longEventsTitle = $"{employeeMetadata.Name} ({@event.Type})";
            var shortEventsTitle = $"{employeeMetadata.Name} ({@event.Type}: {totalHours} hours)";

            var title = @event.Type == CalendarEventTypes.Vacation || @event.Type == CalendarEventTypes.Sickleave
                ? longEventsTitle
                : shortEventsTitle;

            var storageItem = new StorageItem
            {
                Title = title,
                StartDate = @event.Dates.StartDate,
                EndDate = @event.Dates.EndDate,
                Category = @event.Type,
                AllDayEvent = true,
                CalendarEventId = @event.EventId
            };

            return storageItem;
        }

        private bool NeedToStoreCalendarEvent(CalendarEvent @event)
        {
            var calendarEventStatuses = new CalendarEventStatuses();

            var pendingStatuses = calendarEventStatuses.PendingForType(@event.Type);
            var actualStatuses = calendarEventStatuses.ActualForType(@event.Type);

            return actualStatuses.Contains(@event.Status) && !pendingStatuses.Contains(@event.Status);
        }

        private string[] GetSharepointCalendarsByDepartment(string departmentId)
        {
            return this.departmentsCalendarsSettings.DepartmentsCalendars
                .Where(x => x.DepartmentId == departmentId)
                .Select(x => x.Calendar)
                .ToArray();
        }

        private class CalendarEventUpsertSuccess
        {
            public CalendarEventUpsertSuccess(CalendarEvent @event)
            {
                this.Event = @event;
            }

            public CalendarEvent Event { get; }
        }

        private class CalendarEventUpsertFailed
        {
            public CalendarEventUpsertFailed(CalendarEvent @event, Exception exception)
            {
                this.Event = @event;
                this.Exception = exception;
            }

            public CalendarEvent Event { get; }

            public Exception Exception { get; }
        }

        private class CalendarEventRemoveSuccess
        {
            public CalendarEventRemoveSuccess(CalendarEvent @event)
            {
                this.Event = @event;
            }

            public CalendarEvent Event { get; }
        }

        private class CalendarEventRemoveFailed
        {
            public CalendarEventRemoveFailed(CalendarEvent @event, Exception exception)
            {
                this.Event = @event;
                this.Exception = exception;
            }

            public CalendarEvent Event { get; }

            public Exception Exception { get; }
        }
    }
}