namespace Arcadia.Assistant.Web.Download
{
    using System;
    using System.Net.Http;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;

    public class DownloadActor : UntypedActor, ILogReceive
    {
        private readonly IDownloadApplicationSettings downloadApplicationSettings;
        private readonly IHttpClientFactory httpClientFactory;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public DownloadActor(IDownloadApplicationSettings downloadApplicationSettings, IHttpClientFactory httpClientFactory)
        {
            this.downloadApplicationSettings = downloadApplicationSettings;
            this.httpClientFactory = httpClientFactory;

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(downloadApplicationSettings.DownloadBuildIntervalMinutes),
                this.Self,
                RefreshApplicationBuilds.Instance,
                this.Self);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case RefreshApplicationBuilds _:
                    this.GetApplicationBuilds();
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void GetApplicationBuilds()
        {
        }

        private class RefreshApplicationBuilds
        {
            public static readonly RefreshApplicationBuilds Instance = new RefreshApplicationBuilds();
        }
    }
}