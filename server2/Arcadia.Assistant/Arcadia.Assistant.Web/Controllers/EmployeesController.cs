namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;

    using Organization.Contracts;

    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly IOrganization organization;

        public EmployeesController(IOrganization organization)
        {
            this.organization = organization;
        }

        [Route("{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string employeeId, CancellationToken token)
        {
            var employees = await this.organization.FindByIdAsync(employeeId, token);
            return this.Ok(employees);
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FilterEmployees(
            [FromQuery] string departmentId,
            [FromQuery] string roomNumber,
            [FromQuery] string name,
            CancellationToken token)
        {
            var query = EmployeesQuery.Create().ForDepartment(departmentId).ForRoom(roomNumber).WithNameFilter(name);
            var employees = await this.organization.FindEmployeesAsync(query, token);

            return this.Ok(employees.Select(EmployeeModel.FromMetadata).ToArray());
        }
    }
}