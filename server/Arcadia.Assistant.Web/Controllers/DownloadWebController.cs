namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Download;
    using Arcadia.Assistant.Web.Models;

    [Route("")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadWebController : Controller
    {
        private readonly IActorRefFactory actorSystem;
        private readonly ITimeoutSettings timeoutSettings;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IDownloadApplicationSettings downloadApplicationSettings;

        public DownloadWebController(
            IActorRefFactory actorSystem,
            ITimeoutSettings timeoutSettings,
            IHostingEnvironment hostingEnvironment,
            IDownloadApplicationSettings downloadApplicationSettings)
        {
            this.actorSystem = actorSystem;
            this.timeoutSettings = timeoutSettings;
            this.hostingEnvironment = hostingEnvironment;
            this.downloadApplicationSettings = downloadApplicationSettings;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }

        [Route("get/{deviceType}")]
        [HttpGet]
        public async Task<IActionResult> GetFile(string deviceType, CancellationToken token)
        {
            if (!Enum.TryParse(deviceType, out DeviceType appType))
            {
                return this.BadRequest();
            }

            var downloadActor = this.actorSystem.ActorSelection(
                $"/user/{WellKnownActorPaths.DownloadApplicationBuilds}");

            GetLatestApplicationBuildPath.ApplicationTypeEnum getBuildApplicationType;

            if (appType == DeviceType.Android)
            {
                getBuildApplicationType = GetLatestApplicationBuildPath.ApplicationTypeEnum.Android;
            }
            else if (appType == DeviceType.Ios)
            {
                getBuildApplicationType = GetLatestApplicationBuildPath.ApplicationTypeEnum.Ios;
            }
            else
            {
                return this.StatusCode(StatusCodes.Status400BadRequest);
            }

            var message = new GetLatestApplicationBuildPath(getBuildApplicationType);

            var buildPathResponse = await downloadActor.Ask<GetLatestApplicationBuildPath.Response>(
                message, this.timeoutSettings.Timeout, token);

            if (buildPathResponse.Path == null)
            {
                return this.NotFound();
            }

            if (appType == DeviceType.Ios)
            {
                var relativeFilePath = Path
                    .GetRelativePath(this.hostingEnvironment.ContentRootPath, buildPathResponse.Path)
                    .Replace("\\", "/");
                var downloadUrl = this.downloadApplicationSettings.DownloadFileUrl
                    .Replace("{filePath}", relativeFilePath);
                return this.Redirect($"itms-services://?action=download-manifest&url={downloadUrl}");
            }

            return this.PhysicalFile(
                buildPathResponse.Path,
                "application/vnd.android.package-archive",
                Path.GetFileName(buildPathResponse.Path));
        }
    }
}