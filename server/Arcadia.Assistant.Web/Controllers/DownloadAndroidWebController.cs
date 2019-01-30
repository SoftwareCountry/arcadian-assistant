namespace Arcadia.Assistant.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class DownloadAndroidWebController : Controller
    {
        [Route("download/android")]
        [HttpGet]
        public IActionResult Index()
        {
            return this.View();
        }
    }
}