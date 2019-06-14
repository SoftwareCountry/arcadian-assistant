namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;

    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly IEmployees employees;

        public EmployeesController(IEmployees employees)
        {
            this.employees = employees;
        }

        [Route("{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string employeeId, CancellationToken token)
        {
            var employee = await this.employees.FindEmployeeAsync(employeeId, token);
            if (employee == null)
            {
                return this.NotFound();
            }

            return this.Ok(employee);
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
            var employeesList = await this.employees.FindEmployeesAsync(query, token);

            return this.Ok(employeesList.Select(EmployeeModel.FromMetadata).ToArray());
        }
    }
}