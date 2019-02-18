namespace Arcadia.Assistant.CSP.Vacations
{
    using System;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;

    public class CspEmployeeVacationsRegistry : UntypedActor, ILogReceive
    {
        private readonly VacationsSyncExecutor vacationsSyncExecutor;
        private readonly string employeeId;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public CspEmployeeVacationsRegistry(
            VacationsSyncExecutor vacationsSyncExecutor,
            IRefreshInformation refreshInformation,
            string employeeId)
        {
            this.vacationsSyncExecutor = vacationsSyncExecutor;
            this.employeeId = employeeId;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(refreshInformation.IntervalInMinutes),
                this.Self,
                Refresh.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case Refresh _:
                    this.logger.Info($"Updating vacations information for employee {this.employeeId}...");
                    break;

                case RefreshSuccess _:
                    this.logger.Info($"Vacations information for employee {this.employeeId} is updated");
                    break;

                case RefreshFailed msg:
                    this.logger.Error(msg.Exception, $"Failed to load vacations information for employee {this.employeeId}: {msg.Exception.Message}");
                    break;
            }
        }

        private class Refresh
        {
            public static readonly Refresh Instance = new Refresh();
        }

        private class RefreshSuccess
        {
        }

        private class RefreshFailed
        {
            public RefreshFailed(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}