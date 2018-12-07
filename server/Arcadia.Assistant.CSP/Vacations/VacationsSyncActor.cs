namespace Arcadia.Assistant.CSP.Vacations
{
    using System;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Calendar.Abstractions.EventBus;

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
                    this.vacationsSyncExecutor.UpsertVacation(
                            msg.Event,
                            new VacationsSyncExecutor.VacationApprovals(),
                            new VacationsSyncExecutor.VacationsMatchInterval(msg.Event.Dates.StartDate, msg.Event.Dates.EndDate))
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: err => new VacationPersistFailed(err));
                    break;

                case CalendarEventChanged msg:
                    this.vacationsSyncExecutor.UpsertVacation(
                            msg.NewEvent,
                            new VacationsSyncExecutor.VacationApprovals(),
                            new VacationsSyncExecutor.VacationsMatchInterval(msg.OldEvent.Dates.StartDate, msg.OldEvent.Dates.EndDate))
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: err => new VacationPersistFailed(err));
                    break;

                case CalendarEventApprovalsChanged msg:
                    this.vacationsSyncExecutor.UpsertVacation(
                            msg.Event,
                            new VacationsSyncExecutor.VacationApprovals(
                                msg.LastApproverId,
                                msg.LastApprovalDate,
                                msg.Approvals),
                            new VacationsSyncExecutor.VacationsMatchInterval(msg.Event.Dates.StartDate, msg.Event.Dates.EndDate))
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