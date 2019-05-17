namespace Arcadia.Assistant.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class EmployeesController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return this.View();
        }
    }
}