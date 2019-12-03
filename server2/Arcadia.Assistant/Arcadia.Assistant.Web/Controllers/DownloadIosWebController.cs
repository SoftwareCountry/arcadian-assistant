namespace Arcadia.Assistant.Web.Controllers
{
    using System.Text;

    using Microsoft.AspNetCore.Mvc;

    using Models;
    using AppCenterBuilds.Contracts;
    using MobileBuild.Contracts;
    using System.Threading.Tasks;
    using System.Threading;
    using Properties;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadIosWebController : Controller
    {
        private readonly bool sslOffloading;
        private readonly string helpLink;
        private readonly IMobileBuildActorFactory mobileBuildActor;

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
            var version = await mobileBuildActor.MobileBuild(DeviceType.Ios.MobileBuildType()).GetMobileBuildVersionAsync(CancellationToken.None);
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