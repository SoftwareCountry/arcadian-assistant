namespace Arcadia.Assistant.Web.Controllers.Builds
{
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using Configuration;

    using Microsoft.AspNetCore.Mvc;

    using MobileBuild.Contracts;

    using Models;

    using Properties;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadIosWebController : Controller
    {
        private readonly string helpLink;
        private readonly IMobileBuildActorFactory mobileBuildActor;
        private readonly bool sslOffloading;

        public DownloadIosWebController(ISslSettings sslSettings, IHelpSettings helpSettings, IMobileBuildActorFactory mobileBuildActor)
        {
            this.sslOffloading = sslSettings.SslOffloading;
            this.helpLink = helpSettings.HelpLink;
            this.mobileBuildActor = mobileBuildActor;
        }

        [Route("download/ios")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var version = await this.mobileBuildActor.MobileBuild(DeviceType.Ios.MobileBuildType()).GetMobileBuildVersionAsync(CancellationToken.None);
            return this.View(new HomeViewModel
            {
                ManifestLink = this.GetAbsoluteUrl("GetIosManifest", "DownloadIosWeb"),
                HelpLink = this.helpLink,
                Version = version
            });
        }

        [Route("download/ios-manifest")]
        [HttpGet]
        public IActionResult GetIosManifest()
        {
            var getIosFileLink = this.GetAbsoluteUrl("GetIos", "GetFileWeb");

            var manifestContent = Resources.iosManifest
                .Replace("{downloadApplicationUrl}", getIosFileLink);

            return this.File(Encoding.UTF8.GetBytes(manifestContent), "text/html");
        }

        private string GetAbsoluteUrl(string actionName, string controllerName)
        {
            var absoluteUrl = this.sslOffloading
                ? this.Url.Action(actionName, controllerName, null, "https")
                : this.Url.Action(actionName, controllerName, null, this.Request.Scheme);

            return absoluteUrl;
        }
    }
}