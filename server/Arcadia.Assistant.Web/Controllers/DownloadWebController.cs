namespace Arcadia.Assistant.Web.Controllers
{
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Download;

    [Route("/download")]
    public class DownloadWebController : Controller
    {
        private readonly IActorRefFactory actorSystem;
        private readonly ITimeoutSettings timeoutSettings;

        public DownloadWebController(
            IActorRefFactory actorSystem,
            ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.timeoutSettings = timeoutSettings;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [Route("get/{applicationType}")]
        [HttpGet]
        public async Task<IActionResult> GetFile(ApplicationType applicationType, CancellationToken token)
        {
            var downloadActor = this.actorSystem.ActorSelection(
                $"/user/{WellKnownActorPaths.DownloadApplicationBuilds}");

            var getBuildApplicationType = applicationType == ApplicationType.Android
                ? GetLatestApplicationBuildPath.ApplicationTypeEnum.Android
                : GetLatestApplicationBuildPath.ApplicationTypeEnum.Ios;
            var message = new GetLatestApplicationBuildPath(getBuildApplicationType);

            var buildPathResponse = await downloadActor.Ask<GetLatestApplicationBuildPath.Response>(
                message, this.timeoutSettings.Timeout, token);

            if (buildPathResponse.Path == null)
            {
                return this.NotFound();
            }

            var fileContentType = applicationType == ApplicationType.Android
                ? "application/vnd.android.package-archive"
                : "application/octet-stream";

            return this.PhysicalFile(
                buildPathResponse.Path,
                fileContentType,
                Path.GetFileName(buildPathResponse.Path));
        }

        public enum ApplicationType
        {
            Android,
            Ios
        }
    }
}