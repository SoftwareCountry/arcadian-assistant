namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Organization.Abstractions;

    public class ArcadiaVacationCreditRegistry : VacationsCreditRegistry, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly EmployeesQueryExecutor employeesQueryExecutor;
        private readonly IActorRef vacationsEmailLoader;

        private Dictionary<string, double> employeeIdsToDaysLeft = new Dictionary<string, double>();

        private string lastErrorMessage;

        public ArcadiaVacationCreditRegistry(
            EmployeesQueryExecutor employeesQueryExecutor,
            VacationsSyncExecutor vacationsSyncExecutor,
            IRefreshInformation refreshInformation)
        {
            this.employeesQueryExecutor = employeesQueryExecutor;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes),
                this.Self,
                Refresh.Instance,
                this.Self);

            Context.ActorOf(VacationsSyncActor.CreateProps(vacationsSyncExecutor), "vacations-sync");

            this.vacationsEmailLoader = Context.ActorOf(VacationsEmailLoader.CreateProps(), "vacations-email-loader");
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
                    this.LoadEmployeeVacationDays()
                        .PipeTo(
                            this.Self,
                            success: x => new RefreshSuccess(x),
                            failure: err => new RefreshFailed(err));
                    break;

                case RefreshSuccess m:
                    this.logger.Info("Vacations information is updated");
                    this.employeeIdsToDaysLeft = m.EmployeesToDaysLeft;

                    this.lastErrorMessage = null;

                    break;

                case RefreshFailed e:
                    this.logger.Error(e.Exception, $"Failed to load vacations information: {e.Exception.Message}");

                    this.lastErrorMessage = e.Exception.Message;

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<Dictionary<string, double>> LoadEmployeeVacationDays()
        {
            var employeesTask = this.employeesQueryExecutor.Fetch();
            var vacationsInfoTask = this.vacationsEmailLoader
                .Ask<VacationsEmailLoader.GetVacationsInfo.Response>(VacationsEmailLoader.GetVacationsInfo.Instance);

            await Task.WhenAll(employeesTask, vacationsInfoTask);

            if (!(vacationsInfoTask.Result is VacationsEmailLoader.GetVacationsInfo.Success vacationsInfoResult))
            {
                throw new Exception("Unexpected vacations info response");
            }

            this.logger.Debug("Employees and vacations info is loaded");

            var employees = employeesTask.Result.ToList();

            this.logger.Debug($"Employees count: {employees.Count}; Vacations info count: {vacationsInfoResult.EmployeeVacations.Count()}");

            var employeesVacations = employees
                .GroupJoin(
                    vacationsInfoResult.EmployeeVacations,
                    employee => employee.Email.ToLower(),
                    vacation => vacation.Email.ToLower(),
                    (x, y) => new { x.Id, DaysLeft = y.Select(v => v.VacationDaysCount).FirstOrDefault() })
                .ToDictionary(x => x.Id, x => x.DaysLeft);
            return employeesVacations;
        }

        private class Refresh
        {
            public static readonly Refresh Instance = new Refresh();
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
    }
}