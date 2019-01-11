namespace Arcadia.Assistant.Web.Controllers
{
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Download;
    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.Web.Properties;

    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
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

        [Route("get-android")]
        [HttpGet]
        public Task<IActionResult> GetAndroid(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Android, cancellationToken);
        }

        [Route("get-ios", Name = "get-ios")]
        [HttpGet]
        public Task<IActionResult> GetIos(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Ios, cancellationToken);
        }

        [Route("get-ios-manifest", Name = "get-ios-manifest")]
        [HttpGet]
        public IActionResult GetIosManifest(CancellationToken cancellationToken)
        {
            var getIosFileLink = Url.Link("get-ios", null);

            var manifestContent = Resources.IosManifest
                .Replace("{downloadApplicationUrl}", getIosFileLink);

            return this.File(Encoding.UTF8.GetBytes(manifestContent), "text/html");
        }

        public async Task<IActionResult> GetFile(DeviceType deviceType, CancellationToken token)
        {
            var downloadActor = this.actorSystem.ActorSelection(
                $"/user/{WellKnownActorPaths.DownloadApplicationBuilds}");

            var getBuildApplicationType = deviceType == DeviceType.Android
                ? GetLatestApplicationBuildPath.ApplicationTypeEnum.Android
                : GetLatestApplicationBuildPath.ApplicationTypeEnum.Ios;

            var buildPathResponse = await downloadActor.Ask<GetLatestApplicationBuildPath.Response>(
                new GetLatestApplicationBuildPath(getBuildApplicationType),
                this.timeoutSettings.Timeout,
                token);

            if (buildPathResponse.Path == null)
            {
                return this.NotFound();
            }

            var fileContentType = deviceType == DeviceType.Android
                ? "application/vnd.android.package-archive"
                : "application/octet-stream";

            return this.PhysicalFile(
                buildPathResponse.Path,
                fileContentType,
                Path.GetFileName(buildPathResponse.Path));
        }
    }
}