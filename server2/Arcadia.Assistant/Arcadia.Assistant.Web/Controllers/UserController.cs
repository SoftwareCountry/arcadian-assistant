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
    [ApiController]
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserModel>> GetCurrentUser(CancellationToken cancellationToken)
        {
            var userEmployees = await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), cancellationToken);
            var employee = userEmployees.SingleOrDefault();
            if (employee == null)
            {
                return this.Forbid();
            }

            return new UserModel(employee.EmployeeId.ToString(), employee.Email);
        }

        [Route("permissions/{objectEmployeeId}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserEmployeePermissionsModel>> GetPermissions(int objectEmployeeId, CancellationToken token)
        {
            if (this.User.Identity.Name == null)
            {
                return this.Forbid();
            }

            var objectId = new EmployeeId(objectEmployeeId);
            var objectEmployee = await this.employees.FindEmployeeAsync(objectId, token);
            if (objectEmployee == null)
            {
                return this.NotFound();
            }

            var allPermissions = await this.permissions.GetPermissionsAsync(new UserIdentity(this.User.Identity.Name!), token);
            var employeePermissions = allPermissions.GetPermissions(objectEmployee);
            return new UserEmployeePermissionsModel(objectId.ToString(), employeePermissions);
        }
    }
}