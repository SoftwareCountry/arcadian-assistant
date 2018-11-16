namespace Arcadia.Assistant.CSP
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Net;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Health.Abstractions;
    using Microsoft.EntityFrameworkCore;
    using Model;
    using Vacations;

    public class ArcadiaHealthChecker : HealthChecker, ILogReceive
    {
        private const string HealthStateName1C = "1C";
        private const string HealthStateNameDatabase = "Database";

        private readonly Func<ArcadiaCspContext> contextFactory;
        private readonly Remote1CConfiguration bkConfiguration;

        public ArcadiaHealthChecker(Func<ArcadiaCspContext> contextFactory, Remote1CConfiguration bkConfiguration)
        {
            this.contextFactory = contextFactory;
            this.bkConfiguration = bkConfiguration;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case HealthCheckMessage _:
                    var checkDatabaseResult = this.CheckDatabase();

                    this.Check1C().PipeTo(
                        this.Sender,
                        success: check1CResult =>
                        {
                            var healthResult = new Dictionary<string, bool>
                            {
                                { HealthStateName1C, check1CResult },
                                { HealthStateNameDatabase, checkDatabaseResult }
                            };
                            return new HealthCheckMessageResponse(healthResult);
                        });
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<bool> Check1C()
        {
            try
            {
                var uriBuilder = new UriBuilder(this.bkConfiguration.Url);
                var client = new EnterpriseV8(uriBuilder.Uri)
                {
                    Credentials = new NetworkCredential
                    {
                        UserName = this.bkConfiguration.Username,
                        Password = this.bkConfiguration.Password
                    }
                };
                var taskFactory = new TaskFactory();
                await taskFactory.FromAsync(
                    client.Catalog_ВидыОтпусков.BeginExecute,
                    client.Catalog_ВидыОтпусков.EndExecute,
                    new object());
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        private bool CheckDatabase()
        {
            try
            {
                using (var context = this.contextFactory())
                {
                    var connection = context.Database.GetDbConnection();
                    connection.Open();
                    return connection.State == ConnectionState.Open;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}