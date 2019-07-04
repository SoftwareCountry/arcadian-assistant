namespace Arcadia.Assistant.Web.Download
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;

    using Microsoft.AspNetCore.Hosting;

    public class DownloadActor : UntypedActor, ILogReceive, IWithUnboundedStash
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IActorRef androidDownloadBuildActor;
        private readonly IActorRef iosDownloadBuildActor;

        public DownloadActor(
            IDownloadApplicationSettings downloadApplicationSettings,
            IHttpClientFactory httpClientFactory,
            IHostingEnvironment hostingEnvironment,
            ActorPathsBuilder actorPathsBuilder)
        {
            this.androidDownloadBuildActor = Context.ActorOf(
                Props.Create(() => new DownloadApplicationActor(
                    downloadApplicationSettings,
                    httpClientFactory,
                    hostingEnvironment,
                    downloadApplicationSettings.AndroidGetBuildsUrl,
                    downloadApplicationSettings.AndroidGetBuildDownloadLinkTemplateUrl,
                    ApplicationTypeEnum.Android,
                    actorPathsBuilder)),
                "download-android-build");

            this.iosDownloadBuildActor = Context.ActorOf(
                Props.Create(() => new DownloadApplicationActor(
                    downloadApplicationSettings,
                    httpClientFactory,
                    hostingEnvironment,
                    downloadApplicationSettings.IosGetBuildsUrl,
                    downloadApplicationSettings.IosGetBuildDownloadLinkTemplateUrl,
                    ApplicationTypeEnum.Ios,
                    actorPathsBuilder)),
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
                    this.logger.Debug("Refresh application builds started");

                    var androidDownloadTask = this.androidDownloadBuildActor
                        .Ask<DownloadApplicationBuild.Response>(DownloadApplicationBuild.Instance)
                        .PipeTo(
                            this.Self,
                            success: result => this.GetApplicationBuildResult(result, ApplicationTypeEnum.Android),
                            failure: err => new RefreshApplicationBuildError(err));

                    var iosDownloadTask = this.iosDownloadBuildActor
                        .Ask<DownloadApplicationBuild.Response>(DownloadApplicationBuild.Instance)
                        .PipeTo(
                            this.Self,
                            success: result => this.GetApplicationBuildResult(result, ApplicationTypeEnum.Ios),
                            failure: err => new RefreshApplicationBuildError(err));

                    Task.WhenAll(androidDownloadTask, iosDownloadTask)
                        .PipeTo(
                            this.Self,
                            success: () => RefreshApplicationBuildsFinish.Instance);

                    break;

                case RefreshApplicationBuildSuccess msg:
                    this.logger.Debug($"Application build successfully refreshed. Application type: {msg.ApplicationType}, update available: {msg.UpdateAvailable}");

                    if (msg.UpdateAvailable)
                    {
                        Context.System.EventStream.Publish(new UpdateAvailable(msg.ApplicationType));
                    }

                    break;

                case RefreshApplicationBuildError msg:
                    this.logger.Warning(msg.Exception.ToString());
                    break;

                case RefreshApplicationBuildsFinish _:
                    this.logger.Debug("Refresh application builds finished");

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
                case ApplicationTypeEnum.Android:
                    this.androidDownloadBuildActor.Tell(message, this.Sender);
                    break;

                case ApplicationTypeEnum.Ios:
                    this.iosDownloadBuildActor.Tell(message, this.Sender);
                    break;

                default:
                    this.logger.Warning("Not supported application type");
                    break;
            }
        }

        private object GetApplicationBuildResult(DownloadApplicationBuild.Response message, ApplicationTypeEnum applicationType)
        {
            if (message is DownloadApplicationBuild.Error error)
            {
                return new RefreshApplicationBuildError(error.Exception);
            }

            return new RefreshApplicationBuildSuccess(
                applicationType,
                ((DownloadApplicationBuild.Success)message).UpdateAvailable);
        }

        private class RefreshApplicationBuilds
        {
            public static readonly RefreshApplicationBuilds Instance = new RefreshApplicationBuilds();
        }

        private class RefreshApplicationBuildsStart
        {
            public static readonly RefreshApplicationBuildsStart Instance = new RefreshApplicationBuildsStart();
        }

        public class RefreshApplicationBuildSuccess
        {
            public RefreshApplicationBuildSuccess(ApplicationTypeEnum applicationType, bool updateAvailable)
            {
                this.ApplicationType = applicationType;
                this.UpdateAvailable = updateAvailable;
            }

            public ApplicationTypeEnum ApplicationType { get; }

            public bool UpdateAvailable { get; }
        }

        public class RefreshApplicationBuildError
        {
            public RefreshApplicationBuildError(Exception exception)
            {
                this.Exception = exception;
            }

            public Exception Exception { get; }
        }

        public class RefreshApplicationBuildsFinish
        {
            public static readonly RefreshApplicationBuildsFinish Instance = new RefreshApplicationBuildsFinish();
        }
    }
}