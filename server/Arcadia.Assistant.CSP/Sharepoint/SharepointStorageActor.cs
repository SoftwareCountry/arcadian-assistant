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
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class SharepointStorageActor : UntypedActor, ILogReceive
    {
        private const string OrganizationActorPath = @"/user/organization";

        private readonly Func<IExternalStorage> externalStorageProvider;
        private readonly ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings;
        private readonly ActorSelection organizationActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SharepointStorageActor(Func<IExternalStorage> externalStorageProvider, ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings)
        {
            this.externalStorageProvider = externalStorageProvider;
            this.departmentsCalendarsSettings = departmentsCalendarsSettings;
            this.organizationActor = Context.ActorSelection(OrganizationActorPath);

            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRemoved>(this.Self);
        }

        public static Props CreateProps()
        {
            return Context.DI().Props<SharepointStorageActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventRecoverComplete msg:
                    this.OnReceiveEventUpdate(msg.Event);
                    break;

                case CalendarEventChanged msg:
                    this.OnReceiveEventUpdate(msg.NewEvent);
                    break;

                case CalendarEventRemoved msg:
                    this.OnReceiveEventRemove(msg.Event);
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

        private void OnReceiveEventUpdate(CalendarEvent @event)
        {
            if (this.NeedToStoreCalendarEvent(@event))
            {
                this.UpsertCalendarEvent(@event)
                    .PipeTo(
                        this.Self,
                        success: () => new CalendarEventUpsertSuccess(@event),
                        failure: err => new CalendarEventUpsertFailed(@event, err));
            }
            else
            {
                this.RemoveCalendarEvent(@event)
                    .PipeTo(
                        this.Self,
                        success: () => new CalendarEventRemoveSuccess(@event),
                        failure: err => new CalendarEventRemoveFailed(@event, err));
            }
        }

        private void OnReceiveEventRemove(CalendarEvent @event)
        {
            this.RemoveCalendarEvent(@event)
                .PipeTo(
                    this.Self,
                    success: () => new CalendarEventRemoveSuccess(@event),
                    failure: err => new CalendarEventRemoveFailed(@event, err));
        }

        private async Task UpsertCalendarEvent(CalendarEvent @event)
        {
            var employeeMetadata = await this.GetEmployeeMetadata(@event.EmployeeId);
            var departmentCalendars = this.GetSharepointCalendarsByDepartment(employeeMetadata.DepartmentId);

            var externalStorage = this.externalStorageProvider();

            var sharepointTasks = departmentCalendars.Select(async calendar =>
            {
                var existingItem = await this.GetSharepointItemForCalendarEvent(externalStorage, calendar, @event);

                var upsertItem = this.CalendarEventToStorageItem(@event, employeeMetadata);

                if (existingItem == null)
                {
                    await externalStorage.AddItem(
                        calendar,
                        upsertItem);
                }
                else
                {
                    await externalStorage.UpdateItem(
                        calendar,
                        upsertItem);
                }
            });

            await Task.WhenAll(sharepointTasks);
        }

        private async Task RemoveCalendarEvent(CalendarEvent @event)
        {
            var employeeMetadata = await this.GetEmployeeMetadata(@event.EmployeeId);
            var departmentCalendars = this.GetSharepointCalendarsByDepartment(employeeMetadata.DepartmentId);

            var externalStorage = this.externalStorageProvider();

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
            var storageItem = new StorageItem
            {
                Title = $"{employeeMetadata.Name} ({@event.Type})",
                StartDate = @event.Dates.StartDate,
                EndDate = @event.Dates.EndDate,
                Category = @event.Type,
                CalendarEventId = @event.EventId
            };

            if (@event.Type == CalendarEventTypes.Workout)
            {
                storageItem.StartDate = storageItem.StartDate.AddHours(@event.Dates.StartWorkingHour);
                storageItem.EndDate = storageItem.EndDate.AddHours(@event.Dates.FinishWorkingHour);
            }
            else if (@event.Type == CalendarEventTypes.Dayoff)
            {
                storageItem.AllDayEvent = true;
            }

            return storageItem;
        }

        private bool NeedToStoreCalendarEvent(CalendarEvent @event)
        {
            var calendarEventStatuses = new CalendarEventStatuses();

            var pendingStatuses = calendarEventStatuses.PendingForType(@event.Type);
            var actualStatuses = calendarEventStatuses.ActualForType(@event.Type);

            return actualStatuses.Contains(@event.Status) && !pendingStatuses.Contains(@event.Status);
        }

        private async Task<EmployeeMetadata> GetEmployeeMetadata(string employeeId)
        {
            var employeeResponse = await this.organizationActor.Ask<EmployeesQuery.Response>(
                EmployeesQuery.Create().WithId(employeeId));
            return employeeResponse.Employees.First().Metadata;
        }

        private IEnumerable<string> GetSharepointCalendarsByDepartment(string departmentId)
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