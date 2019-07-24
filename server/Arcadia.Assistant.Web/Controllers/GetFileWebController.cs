namespace Arcadia.Assistant.Web.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Akka.Actor;

    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Download;
    using Arcadia.Assistant.Web.Models;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class GetFileWebController : Controller
    {
        private readonly IActorRefFactory actorSystem;
        private readonly ITimeoutSettings timeoutSettings;

        private readonly Dictionary<DeviceType, ApplicationTypeEnum> buildTypeByDeviceType =
            new Dictionary<DeviceType, ApplicationTypeEnum>
            {
                [DeviceType.Android] = ApplicationTypeEnum.Android,
                [DeviceType.Ios] = ApplicationTypeEnum.Ios
            };

        private readonly Dictionary<DeviceType, string> fileContentTypeByDeviceType = new Dictionary<DeviceType, string>
        {
            [DeviceType.Android] = "application/vnd.android.package-archive",
            [DeviceType.Ios] = "application/octet-stream"
        };

        public GetFileWebController(
            IActorRefFactory actorSystem,
            ITimeoutSettings timeoutSettings)
        {
            this.actorSystem = actorSystem;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("get/android")]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public Task<IActionResult> GetAndroid(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Android, cancellationToken);
        }

        [Route("get/ios")]
        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public Task<IActionResult> GetIos(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Ios, cancellationToken);
        }

        private async Task<IActionResult> GetFile(DeviceType deviceType, CancellationToken token)
        {
            var filePath = await this.GetFilePhysicalPath(deviceType, token);
            if (filePath == null)
            {
                return this.NotFound();
            }

            var fileContentType = this.fileContentTypeByDeviceType[deviceType];

            return this.PhysicalFile(
                filePath,
                fileContentType,
                Path.GetFileName(filePath));
        }

        private async Task<string> GetFilePhysicalPath(DeviceType deviceType, CancellationToken token)
        {
            var downloadActor = this.actorSystem.ActorSelection(
                $"/user/{WellKnownActorPaths.DownloadApplicationBuilds}");

            var buildApplicationType = this.buildTypeByDeviceType[deviceType];

            var buildPathResponse = await downloadActor.Ask<GetLatestApplicationBuildPath.Response>(
                new GetLatestApplicationBuildPath(buildApplicationType),
                this.timeoutSettings.Timeout,
                token);

            return buildPathResponse.Path;
        }
    }
}