namespace Arcadia.Assistant.Web.Controllers
{
    using Arcadia.Assistant.Web.Models;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/user")]
    public class UserController : Controller
    {
        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetCurrentUser()
        {
            //TODO: change
            return this.Ok(new UserModel() { EmployeeId = "557", Username = "alexander.shevnin" });
        }
    }
}