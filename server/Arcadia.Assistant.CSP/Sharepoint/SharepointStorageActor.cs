namespace Arcadia.Assistant.CSP.Sharepoint
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.ExternalStorages.Abstractions;
    using Arcadia.Assistant.ExternalStorages.SharepointOnline.Conditions;
    using Arcadia.Assistant.Organization.Abstractions;

    public class SharepointStorageActor : UntypedActor, ILogReceive
    {
        private const string Sdo822CalendarList = "TestCalendar1";

        private readonly Func<IExternalStorage> externalStorageProvider;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public SharepointStorageActor(Func<IExternalStorage> externalStorageProvider)
        {
            this.externalStorageProvider = externalStorageProvider;

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
                case CalendarEventChanged msg:

                    if (this.NeedToStoreCalendarEvent(msg.NewEvent))
                    {
                        this.UpsertCalendarEvent(msg.NewEvent)
                            .PipeTo(
                                this.Self,
                                success: () => new CalendarEventUpsertSuccess(msg.NewEvent),
                                failure: err => new CalendarEventUpsertFailed(msg.NewEvent, err));
                    }
                    else
                    {
                        this.RemoveCalendarEvent(msg.NewEvent)
                            .PipeTo(
                                this.Self,
                                success: () => new CalendarEventRemoveSuccess(msg.NewEvent),
                                failure: err => new CalendarEventRemoveFailed(msg.NewEvent, err));
                    }

                    break;

                case CalendarEventRemoved msg:

                    this.RemoveCalendarEvent(msg.Event)
                        .PipeTo(
                            this.Self,
                            success: () => new CalendarEventRemoveSuccess(msg.Event),
                            failure: err => new CalendarEventRemoveFailed(msg.Event, err));
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

        private async Task UpsertCalendarEvent(CalendarEvent @event)
        {
            var externalStorage = this.externalStorageProvider();

            var existingItem = await this.GetSharepointItemForCalendarEvent(externalStorage, @event);

            var upsertItem = new StorageItem
            {
                Title = $"{@event.EmployeeId} ({@event.Type})",
                StartDate = @event.Dates.StartDate,
                EndDate = @event.Dates.EndDate,
                Category = @event.Type,
                CalendarEventId = @event.EventId
            };

            if (existingItem == null)
            {
                await externalStorage.AddItem(
                    Sdo822CalendarList,
                    upsertItem);
            }
            else
            {
                await externalStorage.UpdateItem(
                    Sdo822CalendarList,
                    upsertItem,
                    new[] { new SharepointEqualCondition(x => x.Id, existingItem.Id) });
            }
        }

        private async Task RemoveCalendarEvent(CalendarEvent @event)
        {
            var externalStorage = this.externalStorageProvider();

            var existingItem = await this.GetSharepointItemForCalendarEvent(externalStorage, @event);

            if (existingItem != null)
            {
                await externalStorage.DeleteItem(
                    Sdo822CalendarList,
                    new[] { new SharepointEqualCondition(x => x.Id, existingItem.Id) });
            }
        }

        private async Task<StorageItem> GetSharepointItemForCalendarEvent(IExternalStorage externalStorage, CalendarEvent @event)
        {
            var existingItems = await externalStorage.GetItems(
                Sdo822CalendarList,
                new[] { new SharepointEqualCondition(x => x.CalendarEventId, @event.EventId) });
            return existingItems.SingleOrDefault();
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
            // ToDo
            return null;
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