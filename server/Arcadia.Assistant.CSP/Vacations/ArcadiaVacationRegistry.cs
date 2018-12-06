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
    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.CSP.Model;
    using Arcadia.Assistant.Organization.Abstractions;

    public class ArcadiaVacationRegistry : VacationsRegistry, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly VacationsQueryExecutor vacationsQueryExecutor;
        private readonly VacationsPersistenceExecutor vacationsPersistenceExecutor;

        private Dictionary<string, double> employeeIdsToDaysLeft = new Dictionary<string, double>();

        private string lastErrorMessage;

        public ArcadiaVacationRegistry(
            VacationsQueryExecutor vacationsQueryExecutor,
            VacationsPersistenceExecutor vacationsPersistenceExecutor,
            IRefreshInformation refreshInformation)
        {
            this.vacationsQueryExecutor = vacationsQueryExecutor;
            this.vacationsPersistenceExecutor = vacationsPersistenceExecutor;

            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes), this.Self, new Refresh(), this.Self);

            Context.System.EventStream.Subscribe<CalendarEventCreated>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventChanged>(this.Self);
            Context.System.EventStream.Subscribe<CalendarEventApprovalsChanged>(this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetVacationInfo request:
                    double value = 0;
                    if (this.employeeIdsToDaysLeft.ContainsKey(request.EmployeeId))
                    {
                        value = this.employeeIdsToDaysLeft[request.EmployeeId];
                    }

                    this.Sender.Tell(new GetVacationInfo.Response((int)Math.Floor(value)));

                    break;

                case GetHealthCheckStatusMessage _:
                    this.Sender.Tell(new GetHealthCheckStatusMessage.GetHealthCheckStatusResponse(this.lastErrorMessage));
                    break;

                case Refresh _:
                    this.logger.Info("Updating vacations information...");
                    this.vacationsQueryExecutor
                        .Fetch()
                        .PipeTo(this.Self, success: x => new RefreshSuccess(x), failure: x => new RefreshFailed(x));
                    break;

                case RefreshSuccess m:
                    this.logger.Info("Vacations registry is updated");
                    this.employeeIdsToDaysLeft = m.EmployeesToDaysLeft;

                    this.lastErrorMessage = null;

                    break;

                case RefreshFailed e:
                    this.logger.Error(e.Exception, $"Failed to load vacations information: {e.Exception.Message}");

                    this.lastErrorMessage = e.Exception.Message;

                    break;

                case CalendarEventCreated msg when msg.Event.Type == CalendarEventTypes.Vacation:
                    this.UpsertVacation(msg.Event, new string[0])
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: e => new VacationPersistFailed(e.Message));
                    break;

                case CalendarEventChanged msg when msg.NewEvent.Type == CalendarEventTypes.Vacation:
                    this.UpsertVacation(msg.NewEvent, new string[0])
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: e => new VacationPersistFailed(e.Message));
                    break;

                case CalendarEventApprovalsChanged msg when msg.Event.Type == CalendarEventTypes.Vacation:
                    this.UpsertVacation(msg.Event, msg.Approvals)
                        .PipeTo(
                            this.Self,
                            success: () => VacationPersistSuccess.Instance,
                            failure: e => new VacationPersistFailed(e.Message));
                    break;

                case VacationPersistSuccess _:
                    this.logger.Debug("Vacation information was updated in the database");
                    break;

                case VacationPersistFailed err:
                    this.logger.Warning($"Failed to update vacation information in the database: {err.Message}");
                    break;

                case CalendarEventCreated _:
                case CalendarEventChanged _:
                case CalendarEventApprovalsChanged _:
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private Task<Vacation> UpsertVacation(CalendarEvent vacationCalendarEvent, IEnumerable<string> approvals)
        {
            var vacation = new Vacation
            {
                EmployeeId = int.Parse(vacationCalendarEvent.EmployeeId),
                RaisedAt = DateTimeOffset.Now,
                Start = vacationCalendarEvent.Dates.StartDate.Date,
                End = vacationCalendarEvent.Dates.EndDate.Date,
                Type = this.GetRegularVacationType()
            };

            if (vacationCalendarEvent.Status == VacationStatuses.Cancelled)
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

            return this.vacationsPersistenceExecutor.UpsertVacation(vacation);
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

        private class Refresh
        {
        }

        private class RefreshSuccess
        {
            public Dictionary<string, double> EmployeesToDaysLeft { get; }

            public RefreshSuccess(Dictionary<string, double> employeesToDaysLeft)
            {
                this.EmployeesToDaysLeft = employeesToDaysLeft;
            }
        }

        private class RefreshFailed
        {
            public Exception Exception { get; }

            public RefreshFailed(Exception exception)
            {
                this.Exception = exception;
            }
        }

        private class VacationPersistSuccess
        {
            public static VacationPersistSuccess Instance = new VacationPersistSuccess();
        }

        private class VacationPersistFailed
        {
            public VacationPersistFailed(string message)
            {
                this.Message = message;
            }

            public string Message { get; }
        }
    }
}