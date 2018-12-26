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

    public class ArcadiaVacationRegistry : VacationsRegistry, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly EmployeesQueryExecutor employeesQueryExecutor;
        private readonly IActorRef vacationsEmailLoader;

        private Dictionary<string, double> employeeIdsToDaysLeft = new Dictionary<string, double>();

        private string lastErrorMessage;

        public ArcadiaVacationRegistry(
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

            if (vacationsInfoTask.Result is VacationsEmailLoader.GetVacationsInfo.Error errorResult)
            {
                throw new Exception("Failed to load employee vacations info", errorResult.Exception);
            }

            var employees = employeesTask.Result;
            var vacationsInfo = ((VacationsEmailLoader.GetVacationsInfo.Success)vacationsInfoTask.Result).EmployeeVacations;

            var employeesVacations = employees
                .GroupJoin(
                    vacationsInfo,
                    employee => employee.Id.ToLower(),
                    vacation => vacation.Id.ToLower(),
                    (x, y) => new { x.Id, DaysLeft = y.Select(v => v.VacationDaysCount).FirstOrDefault() })
                .ToDictionary(x => x.Id.ToString(), x => x.DaysLeft);
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