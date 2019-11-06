namespace Arcadia.Assistant.Web.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.AppCenterBuilds.Contracts;
    using Arcadia.Assistant.MobileBuild.Contracts.Interfaces;
    using System.Text;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class GetFileWebController : Controller
    {
        private readonly IMobileBuildActorFactory mobileBuildActor;

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
            IMobileBuildActorFactory mobileBuildActor)
        {
            this.mobileBuildActor = mobileBuildActor;
        }

        [Route("get/android")]
        [HttpGet]
        public Task<IActionResult> GetAndroid(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Android, cancellationToken);
        }

        [Route("get/ios")]
        [HttpGet]
        public Task<IActionResult> GetIos(CancellationToken cancellationToken)
        {
            return this.GetFile(DeviceType.Ios, cancellationToken);
        }

        private async Task<IActionResult> GetFile(DeviceType deviceType, CancellationToken token)
        {
            var buildApplicationType = this.buildTypeByDeviceType[deviceType];
            var downloadActor = this.mobileBuildActor.MobileBuild(buildApplicationType.ToString());

            try
            {
                var fileContentType = this.fileContentTypeByDeviceType[deviceType];
                var fileContent = await downloadActor.GetMobileBuildDataAsync(CancellationToken.None);
                return this.File(fileContent, fileContentType);
            }
            catch
            {
                return this.NotFound();
            }
        }
    }
}