namespace Arcadia.Assistant.Web.Download
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Web.Configuration;

    public class DownloadActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef androidDownloadBuildActor;
        private readonly IActorRef iosDownloadBuildActor;

        public DownloadActor(
            IDownloadApplicationSettings downloadApplicationSettings,
            IHttpClientFactory httpClientFactory,
            IHostingEnvironment hostingEnvironment)
        {
            this.androidDownloadBuildActor = Context.ActorOf(
                Props.Create(() => new DownloadApplicationActor(
                    downloadApplicationSettings,
                    httpClientFactory,
                    hostingEnvironment,
                    downloadApplicationSettings.AndroidGetBuildsUrl,
                    downloadApplicationSettings.AndroidGetBuildDownloadLinkTemplateUrl)),
                "download-android-build");

            this.iosDownloadBuildActor = Context.ActorOf(
                Props.Create(() => new DownloadApplicationActor(
                    downloadApplicationSettings,
                    httpClientFactory,
                    hostingEnvironment,
                    downloadApplicationSettings.IosGetBuildsUrl,
                    downloadApplicationSettings.IosGetBuildDownloadLinkTemplateUrl)),
                "download-ios-build");

            Context.System.Scheduler.ScheduleTellRepeatedly(
                TimeSpan.Zero,
                TimeSpan.FromMinutes(downloadApplicationSettings.DownloadBuildIntervalMinutes),
                this.Self,
                RefreshApplicationBuilds.Instance,
                this.Self);
        }

        public IStash Stash { get; set; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetLatestApplicationBuildPath msg:
                    this.RespondLatestApplicationBuildPath(msg);
                    break;

                case RefreshApplicationBuilds msg:
                    this.Self.Tell(msg);
                    this.BecomeStacked(this.DownloadingApplicationBuilds);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void DownloadingApplicationBuilds(object message)
        {
            switch (message)
            {
                case RefreshApplicationBuilds _:
                    var androidDownloadTask = this.androidDownloadBuildActor
                        .Ask<DownloadApplicationBuild.Response>(DownloadApplicationBuild.Instance);
                    var iosDownloadTask = this.iosDownloadBuildActor
                        .Ask<DownloadApplicationBuild.Response>(DownloadApplicationBuild.Instance);

                    Task.WhenAll(androidDownloadTask, iosDownloadTask).PipeTo(
                        this.Self,
                        success: result => new RefreshApplicationBuildsFinish(result[0].Message, result[1].Message),
                        failure: err => new RefreshApplicationBuildsFinish(err.Message));

                    break;

                case RefreshApplicationBuildsFinish msg:
                    foreach (var err in msg.Messages.Where(m => m != null))
                    {
                        this.logger.Warning(err);

                    }

                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;

                default:
                    this.Stash.Stash();
                    break;
            }
        }

        private void RespondLatestApplicationBuildPath(GetLatestApplicationBuildPath message)
        {
            switch (message.ApplicationType)
            {
                case GetLatestApplicationBuildPath.ApplicationTypeEnum.Android:
                    this.androidDownloadBuildActor.Tell(message, this.Sender);
                    break;

                case GetLatestApplicationBuildPath.ApplicationTypeEnum.Ios:
                    this.iosDownloadBuildActor.Tell(message, this.Sender);
                    break;

                default:
                    this.logger.Warning("Not supported application type");
                    break;
            }
        }

        private class RefreshApplicationBuilds
        {
            public static readonly RefreshApplicationBuilds Instance = new RefreshApplicationBuilds();
        }

        private class RefreshApplicationBuildsFinish
        {
            public RefreshApplicationBuildsFinish(params string[] messageses)
            {
                this.Messages = messageses;
            }

            public string[] Messages { get; }
        }
    }
}