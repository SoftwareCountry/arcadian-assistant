namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserEmployeeSearch userEmployeeSearch;

        public UserController(IUserEmployeeSearch userEmployeeSearch)
        {
            this.userEmployeeSearch = userEmployeeSearch;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken token)
        {
            var userEmployee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);

            if (userEmployee == null)
            {
                return this.Forbid();
            }

            return this.Ok(new UserModel()
                {
                    EmployeeId = userEmployee.Metadata.EmployeeId,
                    Username = userEmployee.Metadata.Name
                });
        }
    }
}