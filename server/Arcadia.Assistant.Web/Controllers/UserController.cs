namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.Web.Users;
    using Authorization;
    using Employees;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Organization.Abstractions;
    using Organization.Abstractions.OrganizationRequests;
    using Security;

    [Route("api/user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IUserEmployeeSearch userEmployeeSearch;
        private readonly IEmployeesRegistry employeesRegistry;
        private readonly IPermissionsLoader permissionsLoader;

        public UserController(
            IUserEmployeeSearch userEmployeeSearch, 
            IEmployeesRegistry employeesRegistry, 
            IPermissionsLoader permissionsLoader)
        {
            this.userEmployeeSearch = userEmployeeSearch;
            this.employeesRegistry = employeesRegistry;
            this.permissionsLoader = permissionsLoader;
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

        [Route("permissions/{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(UserEmployeePermissionsModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPermissions(string employeeId, CancellationToken token)
        {
            var userEmployee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);

            if (userEmployee == null)
            {
                return this.Forbid();
            }

            var employee = await this.GetEmployeeOrDefaultAsync(employeeId, token);

            if (employee == null)
            {
                return this.NotFound();
            }

            var allPermissions = await this.permissionsLoader.LoadAsync(this.User);
            var employeePermissions = allPermissions.GetPermissions(employee);

            return this.Ok(new UserEmployeePermissionsModel(employeeId, employeePermissions));
        }

        private async Task<EmployeeContainer> GetEmployeeOrDefaultAsync(string employeeId, CancellationToken token)
        {
            var query = new EmployeesQuery().WithId(employeeId);
            var employees = await this.employeesRegistry.SearchAsync(query, token);

            return employees.SingleOrDefault();
        }
    }
}