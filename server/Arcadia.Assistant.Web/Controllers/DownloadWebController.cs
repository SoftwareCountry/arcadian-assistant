namespace Arcadia.Assistant.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;

    [Route("/download")]
    public class DownloadWebController : Controller
    {
        private readonly IDownloadApplicationSettings downloadApplicationSettings;

        public DownloadWebController(IDownloadApplicationSettings downloadApplicationSettings)
        {
            this.downloadApplicationSettings = downloadApplicationSettings;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new DownloadWebModel
            {
                ApkDownloadPath = this.downloadApplicationSettings.ApkDownloadPath,
                iPaDownloadPath = this.downloadApplicationSettings.IpaDownloadPath
            };
            return this.View(model);
        }
    }
}