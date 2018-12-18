namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.IO;
    using System.Linq;

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
                AndroidDownloadPath = this.GetLatestFile(
                    this.downloadApplicationSettings.BuildsFolder,
                    this.downloadApplicationSettings.AndroidFilePattern),
                IosDownloadPath = this.GetLatestFile(
                    this.downloadApplicationSettings.BuildsFolder,
                    this.downloadApplicationSettings.IosFilePattern)
            };

            return this.View(model);
        }

        private string GetLatestFile(string baseFolder, string filePattern)
        {
            var folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, baseFolder);

            if (!Directory.Exists(folder))
            {
                return null;
            }

            var files = Directory
                .GetFiles(folder, filePattern)
                .OrderByDescending(f => f);

            return files.FirstOrDefault();
        }
    }
}