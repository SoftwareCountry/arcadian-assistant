namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;

    using Permissions.Contracts;

    [Route("api/user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IEmployees employees;
        private readonly IPermissions permissions;

        public UserController(IEmployees employees, IPermissions permissions)
        {
            this.employees = employees;
            this.permissions = permissions;
        }

        [HttpGet]
        [ProducesResponseType(typeof(UserModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userEmployees = await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity.Name), cancellationToken);
            var employee = userEmployees.SingleOrDefault();
            if (employee == null)
            {
                return this.Forbid();
            }

            return this.Ok(new UserModel()
            {
                EmployeeId = employee.EmployeeId.ToString(),
                Username = employee.Name
            });
        }

        [Route("permissions/{objectEmployeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserEmployeePermissionsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissions(int objectEmployeeId, CancellationToken token)
        {
            var objectId = new EmployeeId(objectEmployeeId);
            var objectEmployee = await this.employees.FindEmployeeAsync(objectId, token);
            if (objectEmployee == null)
            {
                return this.NotFound();
            }

            var allPermissions = await this.permissions.GetPermissionsAsync(this.User.Identity.Name, token);
            var employeePermissions = allPermissions.GetPermissions(objectEmployee);
            return this.Ok(new UserEmployeePermissionsModel(objectId.ToString(), employeePermissions));
        }
    }
}