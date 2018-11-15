namespace Arcadia.Assistant.Health
{
    using System;
    using System.Data;
    using System.Net;
    using System.Threading.Tasks;

    using Abstractions;
    using Akka.Actor;
    using CSP;
    using CSP.Model;
    using CSP.Vacations;
    using Microsoft.EntityFrameworkCore;

    public class HealthActor : UntypedActor, ILogReceive
    {
        private readonly Func<ArcadiaCspContext> contextFactory;
        private readonly Remote1CConfiguration bkConfiguration;

        public HealthActor(Func<ArcadiaCspContext> contextFactory, Remote1CConfiguration bkConfiguration)
        {
            this.contextFactory = contextFactory;
            this.bkConfiguration = bkConfiguration;
        }

        protected override void OnReceive(object message)
        {
            if (!(message is HealthCheckMessage healthMessage))
            {
                this.Unhandled(message);
                return;
            }

            switch (healthMessage.CheckType)
            {
                case HealthCheckType.Server:
                    this.Sender.Tell(true);
                    break;

                case HealthCheckType.Check1C:
                    this.Check1C().PipeTo(this.Sender);
                    break;

                case HealthCheckType.CheckDatabase:
                    this.Sender.Tell(this.CheckDatabase());
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