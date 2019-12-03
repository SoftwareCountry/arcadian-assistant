namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Configuration;

    using Microsoft.AspNetCore.Mvc;

    using MobileBuild.Contracts;

    using Models;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadAndroidWebController : Controller
    {
        private readonly IHelpSettings helpSettings;
        private readonly IMobileBuildActorFactory mobileBuildActor;

        public DownloadAndroidWebController(IHelpSettings helpSettings, IMobileBuildActorFactory mobileBuildActor)
        {
            this.helpSettings = helpSettings;
            this.mobileBuildActor = mobileBuildActor;
        }

        [Route("download/android")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var version = await this.mobileBuildActor.MobileBuild(DeviceType.Android.MobileBuildType()).GetMobileBuildVersionAsync(CancellationToken.None);
            return this.View(new HomeViewModel
            {
                HelpLink = this.helpSettings.HelpLink,
                Version = version
            });
        }
    }
}