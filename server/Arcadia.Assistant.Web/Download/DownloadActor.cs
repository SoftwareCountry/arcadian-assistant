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
                Props.Create(() => new DownloadAndroidApplicationActor(
                    downloadApplicationSettings,
                    httpClientFactory,
                    hostingEnvironment,
                    downloadApplicationSettings.AndroidGetBuildsUrl,
                    downloadApplicationSettings.AndroidGetBuildDownloadLinkTemplateUrl)),
                "download-android-build");

            this.iosDownloadBuildActor = Context.ActorOf(
                Props.Create(() => new DownloadIosApplicationActor(
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

                case RefreshApplicationBuilds _:
                    this.Self.Tell(RefreshApplicationBuildsStart.Instance);
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
                case RefreshApplicationBuildsStart _:
                    var androidDownloadTask = this.androidDownloadBuildActor
                        .Ask<DownloadApplicationBuild.Response>(DownloadApplicationBuild.Instance);
                    var iosDownloadTask = this.iosDownloadBuildActor
                        .Ask<DownloadApplicationBuild.Response>(DownloadApplicationBuild.Instance);

                    Task.WhenAll(androidDownloadTask, iosDownloadTask).PipeTo(
                        this.Self,
                        success: result =>
                        {
                            var resultExceptions = result
                                .OfType<DownloadApplicationBuild.Error>()
                                .Select(x => x.Exception)
                                .ToList();

                            if (resultExceptions.Count != 0)
                            {
                                var aggregateException = new AggregateException(resultExceptions);
                                return new RefreshApplicationBuildsFinishError(aggregateException);
                            }

                            return RefreshApplicationBuildsFinishSuccess.Instance;
                        },
                        failure: err => new RefreshApplicationBuildsFinishError(err));

                    break;

                case RefreshApplicationBuildsFinishSuccess _:
                    this.Stash.UnstashAll();
                    this.UnbecomeStacked();

                    break;


                case RefreshApplicationBuildsFinishError msg:
                    this.logger.Warning(msg.Exception.Message);

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

        private class RefreshApplicationBuildsStart
        {
            public static readonly RefreshApplicationBuildsStart Instance = new RefreshApplicationBuildsStart();
        }

        public class RefreshApplicationBuildsFinishSuccess
        {
            public static readonly RefreshApplicationBuildsFinishSuccess Instance = new RefreshApplicationBuildsFinishSuccess();
        }

        public class RefreshApplicationBuildsFinishError
        {
            public RefreshApplicationBuildsFinishError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }
    }
}