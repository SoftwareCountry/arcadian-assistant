namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.Organization.Abstractions;

    public class ArcadiaVacationRegistry : VacationsRegistry, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly VacationsQueryExecutor vacationsQueryExecutor;

        private Dictionary<string, double> employeeIdsToDaysLeft = new Dictionary<string, double>();

        private string lastErrorMessage;

        public ArcadiaVacationRegistry(VacationsQueryExecutor vacationsQueryExecutor, IRefreshInformation refreshInformation)
        {
            this.vacationsQueryExecutor = vacationsQueryExecutor;
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.Zero, TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes), this.Self, new Refresh(), this.Self);
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

                case GetVacationRegistryStatusMessage _:
                    this.Sender.Tell(new GetVacationRegistryStatusMessage.GetVacationRegistryStatusResponse(this.lastErrorMessage));
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

                default:
                    this.Unhandled(message);
                    break;
            }
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
    }
}