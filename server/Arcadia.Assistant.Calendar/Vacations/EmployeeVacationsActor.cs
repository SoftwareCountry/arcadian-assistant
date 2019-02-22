namespace Arcadia.Assistant.Calendar.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Patterns;

    public class EmployeeVacationsActor : UntypedActor, ILogReceive
    {
        // Ugly, but it is the simplest way to achieve the goal for now
        private const string CalendarEventsApprovalsCheckerActorPath = @"/user/calendar-events-approvals/calendar-events-approvals-checker";

        private readonly string employeeId;
        private readonly IActorRef employeeFeed;
        private readonly IActorRef vacationsCreditRegistry;
        private readonly ActorSelection calendarEventsApprovalsChecker;
        private readonly IActorRef vacationsRegistry;

        public EmployeeVacationsActor(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsCreditRegistry,
            IEmployeeVacationsRegistryPropsFactory vacationsRegistryPropsFactory
        )
        {
            this.employeeId = employeeId;
            this.employeeFeed = employeeFeed;
            this.vacationsCreditRegistry = vacationsCreditRegistry;

            this.calendarEventsApprovalsChecker = Context.ActorSelection(CalendarEventsApprovalsCheckerActorPath);

            var persistenceSupervisorFactory = new PersistenceSupervisorFactory();

            var vacationsRegistryProps = vacationsRegistryPropsFactory.CreateProps(employeeId);

            this.vacationsRegistry = Context.ActorOf(
                persistenceSupervisorFactory.Get(vacationsRegistryProps),
                "vacations-registry");

            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
        }

        public static Props CreateProps(
            string employeeId,
            IActorRef employeeFeed,
            IActorRef vacationsCreditRegistry,
            IEmployeeVacationsRegistryPropsFactory vacationsRegistryPropsFactory)
        {
            return Props.Create(() => new EmployeeVacationsActor(
                employeeId,
                employeeFeed,
                vacationsCreditRegistry,
                vacationsRegistryPropsFactory)
            );
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventChanged msg when
                    msg.NewEvent.Status == VacationStatuses.Approved &&
                    msg.NewEvent.EmployeeId == this.employeeId:

                    var text = $"Vacation approved from {msg.NewEvent.Dates.StartDate.ToLongDateString()} to {msg.NewEvent.Dates.EndDate.ToLongDateString()}";
                    var feedMessage = new Message(Guid.NewGuid().ToString(), this.employeeId, "Vacation", text, msg.Timestamp.Date);
                    this.employeeFeed.Tell(new PostMessage(feedMessage));

                    break;

                case CalendarEventChanged _:
                    break;

                case GetVacationsCredit _:
                    this.vacationsCreditRegistry
                        .Ask<VacationsCreditRegistry.GetVacationInfo.Response>(new VacationsCreditRegistry.GetVacationInfo(this.employeeId))
                        .ContinueWith(x => new GetVacationsCredit.Response(x.Result.VacationsCredit))
                        .PipeTo(this.Sender);
                    break;

                case GetCalendarEvents msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case GetCalendarEvent msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case GetCalendarEventApprovals msg:
                    this.vacationsRegistry.Forward(msg);
                    break;

                case UpsertCalendarEvent msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            vacation =>
                            {
                                if (vacation == null)
                                {
                                    return new InsertVacation(msg.Event, msg.UpdatedBy, msg.Timestamp);
                                }

                                return new UpdateVacation(msg.Event, vacation, msg.UpdatedBy, msg.Timestamp);
                            },
                            err => new UpsertCalendarEvent.Error(err.Message));
                    break;

                case UpsertCalendarEvent.Error msg:
                    this.Sender.Tell(msg);
                    break;

                case InsertVacation msg:
                    this.InsertVacation(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new InsertVacation.Success(result, msg.CreatedBy, msg.Timestamp),
                            err => new InsertVacation.Error(err));
                    break;

                case InsertVacation.Success msg:
                    Context.System.EventStream.Publish(new CalendarEventCreated(msg.Event, msg.CreatedBy, msg.Timestamp));
                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.Event));
                    break;

                case InsertVacation.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.Message));
                    break;

                case UpdateVacation msg:
                    this.UpdateVacation(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new UpdateVacation.Success(result, msg.OldEvent, msg.UpdatedBy, msg.Timestamp),
                            err => new UpdateVacation.Error(err));
                    break;

                case UpdateVacation.Success msg:
                    Context.System.EventStream.Publish(new CalendarEventChanged(
                        msg.OldEvent,
                        msg.UpdatedBy,
                        msg.Timestamp,
                        msg.NewEvent));
                    this.Sender.Tell(new UpsertCalendarEvent.Success(msg.NewEvent));
                    break;

                case UpdateVacation.Error msg:
                    this.Sender.Tell(new UpsertCalendarEvent.Error(msg.Exception.Message));
                    break;

                case ApproveCalendarEvent msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            vacation =>
                            {
                                if (vacation != null)
                                {
                                    return new ApproveVacation(vacation, msg.ApproverId, msg.Timestamp);
                                }

                                return new ApproveCalendarEvent.ErrorResponse($"Vacation with id {msg.Event.EventId} is not found");
                            },
                            err =>
                            {
                                if (err is ArgumentException)
                                {
                                    return new ApproveCalendarEvent.BadRequestResponse(err.Message);
                                }

                                return new ApproveCalendarEvent.ErrorResponse(err.Message);
                            });
                    break;

                case ApproveCalendarEvent.ErrorResponse msg:
                    this.Sender.Tell(msg);
                    break;

                case ApproveCalendarEvent.BadRequestResponse msg:
                    this.Sender.Tell(msg);
                    break;

                case ApproveVacation msg:
                    this.GrantVacationApproval(msg)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new ApproveVacation.Success(result.Event, result.Approvals),
                            err => new ApproveVacation.Error(err));
                    break;

                case ApproveVacation.Success msg:
                    if (msg.Event != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventApprovalsChanged(msg.Event, msg.Approvals.ToList()));
                    }

                    this.Sender.Tell(ApproveCalendarEvent.SuccessResponse.Instance);

                    break;

                case ApproveVacation.Error msg:
                    this.Sender.Tell(new ApproveCalendarEvent.ErrorResponse(msg.Exception.Message));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<CalendarEvent> GetVacation(string eventId)
        {
            var message = new GetCalendarEvent(eventId);
            var response = await this.vacationsRegistry.Ask<GetCalendarEvent.Response>(message);

            if (response is GetCalendarEvent.Response.Found success)
            {
                return success.Event;
            }

            return null;
        }

        private async Task<IEnumerable<Approval>> GetApprovals(CalendarEvent @event)
        {
            var message = new GetCalendarEventApprovals(@event);
            var response = await this.vacationsRegistry.Ask<GetCalendarEventApprovals.Response>(message);

            switch (response)
            {
                case GetCalendarEventApprovals.SuccessResponse success:
                    return success.Approvals;

                case GetCalendarEventApprovals.ErrorResponse error:
                    throw new Exception(error.Message);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> InsertVacation(InsertVacation message)
        {
            await this.EnsureInsertAvailable(message.Event);

            var response = await this.vacationsRegistry.Ask<InsertVacation.Response>(message);

            switch (response)
            {
                case InsertVacation.Success success:
                    await this.ApproveVacationIfNeeded(success.Event);
                    return success.Event;

                case InsertVacation.Error error:
                    throw new Exception("Error occured on vacation insert", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEvent> UpdateVacation(UpdateVacation message, bool needCheckUpdateAvailable = true)
        {
            // When vacation is changed from internal code (change to Approved when all approvals granted), availability should not be checked
            if (needCheckUpdateAvailable)
            {
                await this.EnsureUpdateAvailable(message.NewEvent, message.OldEvent);
            }

            var response = await this.vacationsRegistry.Ask<UpdateVacation.Response>(message);

            switch (response)
            {
                case UpdateVacation.Success success:
                    return success.NewEvent;

                case UpdateVacation.Error error:
                    throw new Exception("Error occured on vacation update", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task<CalendarEventWithApprovals> GrantVacationApproval(ApproveVacation message)
        {
            this.EnsureApprovalAvailable(message.Event);

            var response = await this.vacationsRegistry.Ask<ApproveVacation.Response>(message);

            switch (response)
            {
                case ApproveVacation.Success success:
                    await this.ApproveVacationIfNeeded(success.Event, success.Approvals, message.ApprovedBy, message.Timestamp);
                    return new CalendarEventWithApprovals(success.Event, success.Approvals);

                case ApproveVacation.Error error:
                    throw new Exception("Error occured on vacation approval granted", error.Exception);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        private async Task ApproveVacationIfNeeded(
            CalendarEvent @event,
            IEnumerable<Approval> approvals = null,
            string approvedBy = null,
            DateTimeOffset? timestamp = null)
        {
            var nextApproverId = await this.GetNextApproverId(@event, approvals ?? Enumerable.Empty<Approval>());
            if (nextApproverId != null)
            {
                return;
            }

            var approvedStatus = new CalendarEventStatuses().ApprovedForType(@event.Type);
            var newEvent = new CalendarEvent(
                @event.EventId,
                @event.Type,
                @event.Dates,
                approvedStatus,
                @event.EmployeeId
            );

            // If there is no approvals, then employee is Director General and event is updated by himself
            approvedBy = approvedBy ?? @event.EmployeeId;
            timestamp = timestamp ?? DateTimeOffset.Now;

            var message = new UpdateVacation(newEvent, @event, approvedBy, (DateTimeOffset)timestamp);
            await this.UpdateVacation(message, false);
        }

        private async Task<string> GetNextApproverId(CalendarEvent @event, IEnumerable<Approval> approvals)
        {
            var approvedBy = approvals.Select(a => a.ApprovedBy);

            var response = await this.calendarEventsApprovalsChecker
                .Ask<GetNextCalendarEventApprover.Response>(
                    new GetNextCalendarEventApprover(@event.EmployeeId, approvedBy, @event.Type));

            switch (response)
            {
                case GetNextCalendarEventApprover.SuccessResponse msg:
                    return msg.NextApproverEmployeeId;

                case GetNextCalendarEventApprover.ErrorResponse msg:
                    throw new Exception(msg.Message);

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
                    "is not allowed for vacation");
            }

            await this.EnsureDatesAreNotIntersected(newEvent);

            if (oldEvent.Dates != newEvent.Dates)
            {
                if (!oldEvent.IsPending)
                {
                    throw new Exception($"Date change is not allowed in status {oldEvent.Status} for {oldEvent.Type}");
                }

                var approvals = await this.GetApprovals(oldEvent);
                if (approvals.Count() != 0)
                {
                    throw new Exception($"Date change is not allowed when there is at least one user approval for {oldEvent.Type}");
                }
            }
        }

        private void EnsureApprovalAvailable(CalendarEvent @event)
        {
            if (!@event.IsPending)
            {
                throw new ArgumentException($"Approval of non-pending event with id {@event.EventId} is not allowed");
            }
        }

        private async Task EnsureDatesAreNotIntersected(CalendarEvent @event)
        {
            var message = new CheckDatesAvailability(@event);
            var response = await this.vacationsRegistry.Ask<CheckDatesAvailability.Response>(message);

            switch (response)
            {
                case CheckDatesAvailability.Success success:
                    if (!success.Result)
                    {
                        throw new Exception($"Event {@event.EventId}. Dates intersect with another actual vacation");
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
            return VacationStatuses.Requested;
        }

        private bool IsStatusTransitionAllowed(string oldCalendarEventStatus, string newCalendarEventStatus)
        {
            return VacationStatuses.All.Contains(newCalendarEventStatus)
                && oldCalendarEventStatus != VacationStatuses.Cancelled
                && oldCalendarEventStatus != VacationStatuses.Rejected
                && oldCalendarEventStatus != VacationStatuses.Processed
                && newCalendarEventStatus != VacationStatuses.Requested
                && newCalendarEventStatus != VacationStatuses.Approved
                && newCalendarEventStatus != VacationStatuses.Processed;
        }

        private class CalendarEventWithApprovals
        {
            public CalendarEventWithApprovals(CalendarEvent @event, IEnumerable<Approval> approvals)
            {
                this.Event = @event;
                this.Approvals = approvals;
            }

            public CalendarEvent Event { get; }

            public IEnumerable<Approval> Approvals { get; }
        }
    }
}