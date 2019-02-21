namespace Arcadia.Assistant.CSP.Vacations
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

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly string employeeId;

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.employeeId = employeeId;

            Context.System.EventStream.Subscribe<CalendarEventRemovedFromApprovers>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetCalendarEvents _:
                    this.GetVacations()
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                var vacations = result.Select(x => x.CalendarEvent).ToList();
                                return new GetCalendarEvents.Response(this.employeeId, vacations);
                            });
                    break;


                case GetCalendarEvent msg:
                    this.GetVacation(msg.EventId)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    return new GetCalendarEvent.Response.Found(result.CalendarEvent);
                                }

                                return new GetCalendarEvent.Response.NotFound();
                            },
                            failure: err => new GetCalendarEvent.Response.NotFound());
                    break;

                case GetCalendarEventApprovals msg:
                    this.GetVacation(msg.Event.EventId)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    return new GetCalendarEventApprovals.SuccessResponse(result.Approvals.ToList());
                                }

                                return new GetCalendarEventApprovals.ErrorResponse($"Vacation with id {msg.Event.EventId} is not found");
                            },
                            failure: err => new GetCalendarEventApprovals.ErrorResponse(err.Message));
                    break;

                case InsertVacation msg:
                    this.vacationsSyncExecutor.InsertVacation(msg.Event, msg.Timestamp, msg.CreatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result => new InsertVacation.Success(result.CalendarEvent, msg.CreatedBy, msg.Timestamp),
                            failure: err => new InsertVacation.Error(err));
                    break;

                case UpdateVacation msg:
                    this.vacationsSyncExecutor.UpdateVacation(msg.NewEvent, msg.Timestamp, msg.UpdatedBy)
                        .PipeTo(
                            this.Sender,
                            success: result => new UpdateVacation.Success(result.CalendarEvent, msg.OldEvent, msg.UpdatedBy, msg.Timestamp),
                            failure: err => new UpdateVacation.Error(err));
                    break;

                case ApproveVacation msg:
                    this.ApproveVacation(msg)
                        .PipeTo(
                            this.Sender,
                            success: result =>
                            {
                                if (result != null)
                                {
                                    return new ApproveVacation.Success(result.CalendarEvent, result.Approvals.ToList());
                                }

                                return Calendar.Abstractions.EmployeeVacations.ApproveVacation.Success.Instance;
                            },
                            failure: err => new ApproveVacation.Error(err));
                    break;

                case CalendarEventRemovedFromApprovers msg:
                    this.ApproveCalendarEvent(msg.Event.EventId)
                        .PipeTo(
                            this.Self,
                            this.Sender,
                            result => new CalendarEventApproved(result));
                    break;

                case CalendarEventApproved msg:
                    if (msg.Data != null)
                    {
                        Context.System.EventStream.Publish(new CalendarEventChanged(
                            msg.Data.OldEvent,
                            msg.Data.UpdatedBy,
                            msg.Data.Timestamp,
                            msg.Data.NewEvent));
                    }

                    break;

                case CheckDatesAvailability msg:
                    var datesAvailable = this.CheckDatesAvailability(msg.Event);
                    this.Sender.Tell(new CheckDatesAvailability.Response(datesAvailable));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<IEnumerable<CalendarEventWithApprovals>> GetVacations()
        {
            var vacations = await this.vacationsSyncExecutor.GetVacations(this.employeeId);
            return vacations
                .Where(v => this.IsCalendarEventActual(v.CalendarEvent))
                .ToList();
        }

        private Task<CalendarEventWithApprovals> GetVacation(string eventId)
        {
            return this.vacationsSyncExecutor.GetVacation(this.employeeId, eventId);
        }

        private async Task<CalendarEventWithApprovals> ApproveVacation(ApproveVacation message)
        {
            var vacation = await this.GetVacation(message.Event.EventId);
            if (vacation == null)
            {
                throw new Exception($"Vacation with id {message.Event.EventId} is not found");
            }

            var calendarEvent = vacation.CalendarEvent;
            var approvals = vacation.Approvals;

            if (approvals.Any(a => a.ApprovedBy == message.ApprovedBy))
            {
                return null;
            }

            await this.vacationsSyncExecutor.UpsertVacationApproval(calendarEvent, message.Timestamp, message.ApprovedBy);

            var newEvent = await this.GetVacation(calendarEvent.EventId);
            return newEvent;
        }

        private async Task<CalendarEventApprovedData> ApproveCalendarEvent(string eventId)
        {
            var oldVacation = await this.GetVacation(eventId);

            var oldEvent = oldVacation?.CalendarEvent;
            if (oldEvent?.IsPending != true)
            {
                return null;
            }

            var approvedStatus = new CalendarEventStatuses().ApprovedForType(oldEvent.Type);
            var newEvent = new CalendarEvent(
                oldEvent.EventId,
                oldEvent.Type,
                oldEvent.Dates,
                approvedStatus,
                oldEvent.EmployeeId
            );

            var approvals = oldVacation.Approvals;

            var lastApproval = approvals
                .OrderByDescending(a => a.Timestamp)
                .FirstOrDefault();

            // If there is no approvals, then employee is Director General and event is updated by himself
            var updatedBy = lastApproval?.ApprovedBy ?? newEvent.EmployeeId;
            var timestamp = lastApproval?.Timestamp ?? DateTimeOffset.Now;

            var newVacation = await this.vacationsSyncExecutor.UpdateVacation(newEvent, timestamp, updatedBy);

            return new CalendarEventApprovedData(oldEvent, newVacation.CalendarEvent, updatedBy, timestamp);
        }

        private bool CheckDatesAvailability(CalendarEvent @event)
        {
            return true;
            //var intersectedEventExists = this.eventsById.Values
            //    .Where(ev => ev.EventId != @event.EventId)
            //    .Any(ev => ev.Dates.DatesIntersectsWith(@event.Dates));
            //return !intersectedEventExists;
        }

        private bool IsCalendarEventActual(CalendarEvent @event)
        {
            return VacationStatuses.Actual.Contains(@event.Status);
        }

        private class CalendarEventApprovedData
        {
            public CalendarEventApprovedData(
                CalendarEvent oldEvent,
                CalendarEvent newEvent,
                string updatedBy,
                DateTimeOffset timestamp)
            {
                this.OldEvent = oldEvent;
                this.NewEvent = newEvent;
                this.UpdatedBy = updatedBy;
                this.Timestamp = timestamp;
            }

            public CalendarEvent OldEvent { get; }

            public CalendarEvent NewEvent { get; }

            public string UpdatedBy { get; }

            public DateTimeOffset Timestamp { get; }

        }

        private class CalendarEventApproved
        {
            public CalendarEventApproved(CalendarEventApprovedData data)
            {
                this.Data = data;
            }

            public CalendarEventApprovedData Data { get; }
        }
    }
}