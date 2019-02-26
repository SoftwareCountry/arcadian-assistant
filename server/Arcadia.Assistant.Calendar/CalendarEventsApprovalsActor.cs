namespace Arcadia.Assistant.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.Calendar.Abstractions.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class CalendarEventsApprovalsActor : UntypedActor, ILogReceive
    {
        private const string OrganizationActorPath = @"/user/organization";

        private readonly IActorRef calendarEventsApprovalsChecker;
        private readonly ActorSelection organizationActor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public CalendarEventsApprovalsActor()
        {
            this.calendarEventsApprovalsChecker = Context.ActorOf(CalendarEventsApprovalsChecker.GetProps(), "calendar-events-approvals-checker");
            this.organizationActor = Context.ActorSelection(OrganizationActorPath);

            Context.System.EventStream.Subscribe<CalendarEventCreated>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventRecoverComplete>(this.Self);
        }

        public static Props GetProps()
        {
            return Props.Create<CalendarEventsApprovalsActor>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventCreated msg:
                    this.OnCalendarEventNeedNextApprover(msg.Event);
                    break;

                case CalendarEventChanged msg:
                    this.OnCalendarEventNeedNextApprover(msg.NewEvent);
                    break;

                case CalendarEventApprovalsChanged msg:
                    this.OnCalendarEventNeedNextApprover(msg.Event);
                    break;

                case CalendarEventRecoverComplete msg when msg.Event.IsPending:
                    this.OnCalendarEventRecoverComplete(msg.Event);
                    break;

                case CalendarEventRecoverComplete _:
                    break;

                case GetNextApproverSuccess msg:
                    if (msg.NextApproverId != null)
                    {
                        if (!msg.CalendarEventRecoverComplete)
                        {
                            Context.System.EventStream.Publish(new CalendarEventAssignedToApprover(msg.Event, msg.NextApproverId));
                        }

                        Context.System.EventStream.Publish(new CalendarEventAddedToPendingActions(msg.Event, msg.NextApproverId));
                    }
                    else if (!msg.CalendarEventRecoverComplete)
                    {
                        Context.System.EventStream.Publish(new CalendarEventRemovedFromApprovers(msg.Event));
                        Context.System.EventStream.Publish(new CalendarEventRemovedFromPendingActions(msg.Event));
                    }

                    break;

                case GetNextApproverError msg:
                    this.logger.Warning($"Failed to get next approver for the event {msg.EventId}. Error: {msg.Message}");
                    break;


                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnCalendarEventNeedNextApprover(CalendarEvent @event)
        {
            if (!@event.IsPending)
            {
                Context.System.EventStream.Publish(new CalendarEventRemovedFromApprovers(@event));
                return;
            }

            this.GetNextApproverId(@event)
                .PipeTo(
                    this.Self,
                    success: nextApproverId => new GetNextApproverSuccess(@event, nextApproverId),
                    failure: err => new GetNextApproverError(@event.EventId, err.ToString())
                );
        }

        private void OnCalendarEventRecoverComplete(CalendarEvent @event)
        {
            if (!@event.IsPending)
            {
                return;
            }

            this.GetNextApproverId(@event)
                .PipeTo(
                    this.Self,
                    success: nextApproverId => new GetNextApproverSuccess(@event, nextApproverId, true),
                    failure: err => new GetNextApproverError(@event.EventId, err.ToString())
                );
        }

        private async Task<string> GetNextApproverId(CalendarEvent @event)
        {
            var employee = await this.GetEmployee(@event.EmployeeId);
            var approvals = await this.GetEmployeeApprovals(employee, @event);
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

        private async Task<EmployeeContainer> GetEmployee(string employeeId)
        {
            var query = EmployeesQuery.Create().WithId(employeeId);
            var response = await this.organizationActor.Ask<EmployeesQuery.Response>(query);

            return response.Employees.FirstOrDefault();
        }

        private async Task<IEnumerable<Approval>> GetEmployeeApprovals(EmployeeContainer employee, CalendarEvent @event)
        {
            var message = new GetCalendarEventApprovals(@event);
            var response = await employee.Calendar.VacationsActor.Ask<GetCalendarEventApprovals.Response>(message);

            switch (response)
            {
                case GetCalendarEventApprovals.SuccessResponse msg:
                    return msg.Approvals;

                case GetCalendarEventApprovals.ErrorResponse msg:
                    throw new Exception(msg.Message);

                default:
                    throw new Exception("Not expected response type");
            }
        }

        public class GetNextApproverSuccess
        {
            public GetNextApproverSuccess(CalendarEvent @event, string nextApproverId, bool calendarEventRecoverComplete = false)
            {
                this.Event = @event;
                this.NextApproverId = nextApproverId;
                this.CalendarEventRecoverComplete = calendarEventRecoverComplete;
            }

            public CalendarEvent Event { get; }

            public string NextApproverId { get; }

            public bool CalendarEventRecoverComplete { get; }
        }

        public class GetNextApproverError
        {
            public GetNextApproverError(string eventId, string message)
            {
                this.EventId = eventId;
                this.Message = message;
            }

            public string EventId { get; }

            public string Message { get; }
        }
    }
}