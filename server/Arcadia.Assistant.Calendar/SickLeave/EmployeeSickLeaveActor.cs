namespace Arcadia.Assistant.Calendar.SickLeave
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeSickLeaves;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Patterns;

    public class EmployeeSickLeaveActor : UntypedActor, ILogReceive
    {
        private readonly IActorRef sickLeavesRegistry;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public EmployeeSickLeaveActor(
            string employeeId,
            IEmployeeSickLeavesRegistryPropsFactory sickLeavesRegistryPropsFactory)
        {
            var persistenceSupervisorFactory = new PersistenceSupervisorFactory();

            var sickLeavesRegistryProps = sickLeavesRegistryPropsFactory.CreateProps(employeeId);

            this.sickLeavesRegistry = Context.ActorOf(
                persistenceSupervisorFactory.Get(sickLeavesRegistryProps),
                "sick-leaves-registry");
        }

        public static Props CreateProps(
            string employeeId,
            IEmployeeSickLeavesRegistryPropsFactory sickLeavesRegistryPropsFactory)
        {
            return Props.Create(() => new EmployeeSickLeaveActor(employeeId, sickLeavesRegistryPropsFactory));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetCalendarEvents msg:
                    this.sickLeavesRegistry.Forward(msg);
                    break;

                case GetCalendarEvent msg:
                    this.sickLeavesRegistry.Forward(msg);
                    break;

                case GetCalendarEventApprovals _:
                    this.Sender.Tell(new GetCalendarEventApprovals.SuccessResponse(new Approval[0]));
                    break;

                case UpsertCalendarEvent msg:
                    this.GetSickLeave(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            sickLeave =>
                            {
                                if (sickLeave == null)
                                {
                                    return new InsertSickLeave(msg.Event, msg.UpdatedBy, msg.Timestamp);
                                }

                                return new UpdateSickLeave(msg.Event, sickLeave, msg.UpdatedBy, msg.Timestamp);
                            },
                            err => new UpsertCalendarEvent.Error(err.ToString()));
                    break;

                case UpsertCalendarEvent.Error msg:
                    this.Sender.Tell(msg);
                    break;

                case InsertSickLeave msg:
                    this.InsertSickLeave(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new InsertSickLeaveSuccess(result),
                            err => new InsertSickLeave.Error(err));
                    break;

                case InsertSickLeaveSuccess msg:
                    this.logger.Debug($"Sick leave {msg.Data.Event.EventId} is created.");

                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Data.Event, msg.Data.CreatedBy, msg.Data.Timestamp));
                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.Data.Event));

                    break;

                case InsertSickLeave.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                case UpdateSickLeave msg:
                    this.UpdateSickLeave(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new UpdateSickLeave.Success(result, msg.OldEvent, msg.UpdatedBy, msg.Timestamp),
                            err => new UpdateSickLeave.Error(err));
                    break;

                case UpdateSickLeave.Success msg:
                    this.logger.Debug($"Sick leave {msg.NewEvent.EventId} is changed.");

                    Context.System.EventStream.Publish(new CalendarEventChanged(
                        msg.OldEvent,
                        msg.UpdatedBy,
                        msg.Timestamp,
                        msg.NewEvent));

                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.NewEvent));

                    break;

                case UpdateSickLeave.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.ToString()));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<CalendarEvent> GetSickLeave(string eventId)
        {
            var message = new GetCalendarEvent(eventId);
            var response = await this.sickLeavesRegistry.Ask<GetCalendarEvent.Response>(message);

            if (response is GetCalendarEvent.Response.Found success)
            {
                return success.Event;
            }

            return null;
        }

        private async Task<InsertSickLeaveSuccessData> InsertSickLeave(InsertSickLeave message)
        {
            await this.EnsureInsertAvailable(message.Event);

            var response = await this.sickLeavesRegistry.Ask<InsertSickLeave.Response>(message);

            switch (response)
            {
                case InsertSickLeave.Success success:
                    return new InsertSickLeaveSuccessData(success.Event, message.CreatedBy, message.Timestamp);

                case InsertSickLeave.Error error:
                    throw new Exception("Error occured on sick leave insert", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> UpdateSickLeave(UpdateSickLeave message)
        {
            await this.EnsureUpdateAvailable(message.NewEvent, message.OldEvent);

            var response = await this.sickLeavesRegistry.Ask<UpdateSickLeave.Response>(message);

            switch (response)
            {
                case UpdateSickLeave.Success success:
                    return success.NewEvent;

                case UpdateSickLeave.Error error:
                    throw new Exception("Error occured on sick leave update", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task EnsureInsertAvailable(CalendarEvent @event)
        {
            if (@event.Status != this.GetInitialStatus())
            {
                throw new Exception($"Event {@event.EventId}. Initial status must be {this.GetInitialStatus()}");
            }

            await this.EnsureDatesAreNotIntersected(@event);
        }

        private async Task EnsureUpdateAvailable(CalendarEvent newEvent, CalendarEvent oldEvent)
        {
            if (oldEvent.Status != newEvent.Status && !this.IsStatusTransitionAllowed(oldEvent.Status, newEvent.Status))
            {
                throw new Exception(
                    $"Event {newEvent.EventId}. Status transition {oldEvent.Status} -> {newEvent.Status} " +
                    "is not allowed for sick leave");
            }

            if (oldEvent.Dates.StartDate != newEvent.Dates.StartDate)
            {
                throw new Exception("Start date cannot be changed");
            }

            await this.EnsureDatesAreNotIntersected(newEvent);
        }

        private async Task EnsureDatesAreNotIntersected(CalendarEvent @event)
        {
            var message = new CheckDatesAvailability(@event);
            var response = await this.sickLeavesRegistry.Ask<CheckDatesAvailability.Response>(message);

            switch (response)
            {
                case CheckDatesAvailability.Success success:

                    if (!success.Result)
                    {
                        throw new Exception($"Event {@event.EventId}. Dates intersect with another actual sick leave");
                    }

                    return;

                case CheckDatesAvailability.Error error:
                    throw new Exception("Error occured on dates availability check", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private string GetInitialStatus()
        {
            return SickLeaveStatuses.Requested;
        }

        private bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return SickLeaveStatuses.All.Contains(newCalendarEventStatus)
                && oldCalendarEventStatus != SickLeaveStatuses.Cancelled
                && oldCalendarEventStatus != SickLeaveStatuses.Completed
                && newCalendarEventStatus != SickLeaveStatuses.Requested;
        }

        private class InsertSickLeaveSuccessData
        {
            public InsertSickLeaveSuccessData(CalendarEvent @event, string createdBy, DateTimeOffset timestamp)
            {
                this.Event = @event;
                this.CreatedBy = createdBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent Event { get; }

            public string CreatedBy { get; }

            public DateTimeOffset Timestamp { get; }
        }

        private class InsertSickLeaveSuccess
        {
            public InsertSickLeaveSuccess(InsertSickLeaveSuccessData data)
            {
                this.Data = data;
            }

            public InsertSickLeaveSuccessData Data { get; }
        }
    }
}