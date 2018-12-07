namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;
    using Arcadia.Assistant.CSP.Model;

    public class VacationsSyncActor : UntypedActor, ILogReceive
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public VacationsSyncActor(VacationsSyncExecutor vacationsSyncExecutor)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;

            Context.System.EventStream.Subscribe<CalendarEventCreated>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
        }

        public static Props CreateProps(VacationsSyncExecutor vacationsSyncExecutor)
        {
            return Props.Create(() => new VacationsSyncActor(vacationsSyncExecutor));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case CalendarEventCreated createdMsg when createdMsg.Event.Type != CalendarEventTypes.Vacation:
                case CalendarEventChanged changedMsg when changedMsg.NewEvent.Type != CalendarEventTypes.Vacation:
                case CalendarEventApprovalsChanged approvedMsg when approvedMsg.Event.Type != CalendarEventTypes.Vacation:
                    break;

                case CalendarEventCreated msg:
                    this.UpsertVacation(msg.Event, null, new string[0])
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: err => new VacationPersistFailed(err));
                    break;

                case CalendarEventChanged msg:
                    this.UpsertVacation(msg.NewEvent, msg.OldEvent, new string[0])
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: err => new VacationPersistFailed(err));
                    break;

                case CalendarEventApprovalsChanged msg:
                    this.UpsertVacation(msg.Event, null, msg.Approvals)
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: err => new VacationPersistFailed(err));
                    break;

                case VacationPersistSuccess _:
                    this.logger.Debug("Vacation information was updated in the database");
                    break;

                case VacationPersistFailed err:
                    this.logger.Error(err.Exception, $"Failed to update vacation information in the database: {err.Exception.Message}");
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private Task<Vacation> UpsertVacation(
            CalendarEvent @event,
            CalendarEvent oldEvent,
            IEnumerable<string> approvals)
        {
            var vacation = this.GetVacationFromCalendarEvent(@event);
            var oldVacation = this.GetVacationFromCalendarEvent(oldEvent);

            if (@event.Status == VacationStatuses.Cancelled)
            {
                vacation.CancelledAt = DateTimeOffset.Now;
            }

            var vacationApprovals = approvals
                .Select(a => new VacationApproval
                {
                    ApproverId = int.Parse(a),
                    TimeStamp = DateTimeOffset.Now,
                    Status = this.GetApprovalApprovedStatus()
                });

            foreach (var vacationApproval in vacationApprovals)
            {
                vacation.VacationApprovals.Add(vacationApproval);
            }

            return this.vacationsSyncExecutor.SyncVacation(vacation, oldVacation);
        }

        private Vacation GetVacationFromCalendarEvent(CalendarEvent @event)
        {
            if (@event == null)
            {
                return null;
            }

            return new Vacation
            {
                EmployeeId = int.Parse(@event.EmployeeId),
                RaisedAt = DateTimeOffset.Now,
                Start = @event.Dates.StartDate.Date,
                End = @event.Dates.EndDate.Date,
                Type = this.GetRegularVacationType()
            };
        }

        // ToDo: get it from database
        private int GetRegularVacationType()
        {
            return 0;
        }

        // ToDo: get it from database
        private int GetApprovalApprovedStatus()
        {
            return 2;
        }

        private class VacationPersistSuccess
        {
            public static readonly VacationPersistSuccess Instance = new VacationPersistSuccess();
        }

        private class VacationPersistFailed
        {
            public VacationPersistFailed(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}