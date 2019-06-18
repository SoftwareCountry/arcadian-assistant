namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IEmployees employees;

        public UserController(IEmployees employees)
        {
            this.employees = employees;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userEmployees = await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity.Name), cancellationToken);
            var employee = userEmployees.SingleOrDefault();
            if (employee == null)
            {
                return this.Forbid();
            }

            return this.Ok(employee);
        }
    }
}