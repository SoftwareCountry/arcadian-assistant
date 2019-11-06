namespace Arcadia.Assistant.Web.Controllers
{
    using Arcadia.Assistant.AppCenterBuilds.Contracts.Interfaces;
    using Arcadia.Assistant.Web.Models;
    using Microsoft.AspNetCore.Mvc;

    [ApiExplorerSettings(IgnoreApi = true)]
    public class DownloadAndroidWebController : Controller
    {
        private readonly IHelpSettings helpSettings;

        public DownloadAndroidWebController(IHelpSettings helpSettings)
        {
            this.helpSettings = helpSettings;
        }

        [Route("download/android")]
        [HttpGet]
        public IActionResult Index()
        {
            return this.View(new HomeViewModel
            {
                HelpLink = this.helpSettings.HelpLink
            });
        }
    }
}