namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Models;

    using Microsoft.AspNetCore.Http;

    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private IEmployeesSearch employeesSearch;

        private ITimeoutSettings timeoutSettings;

        public EmployeesController(IEmployeesSearch employeesSearch, ITimeoutSettings timeoutSettings)
        {
            this.employeesSearch = employeesSearch;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("{employeeId}")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string employeeId, CancellationToken token)
        {
            var employees = await this.LoadEmployeesAsync(new EmployeesQuery().WithId(employeeId), token);
            if (employees.Length == 0)
            {
                return this.NotFound();
            }

            return this.Ok(employees.Single());
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(EmployeeModel[]), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForDepartment([FromQuery] string departmentId, [FromQuery] string roomNumber, CancellationToken token)
        {
            if (this.Request.Query.Count == 0)
            {
                return this.BadRequest("At least one query parameter must be specified");
            }

            var query = new EmployeesQuery();
            if (!string.IsNullOrWhiteSpace(departmentId))
            {
                query = query.ForDepartment(departmentId);
            }

            if (!string.IsNullOrWhiteSpace(roomNumber))
            {
                query = query.ForRoom(roomNumber);
            }

            var employees = await this.LoadEmployeesAsync(query, token);

            return this.Ok(employees);
        }

        private async Task<EmployeeModel[]> LoadEmployeesAsync(EmployeesQuery query, CancellationToken token)
        {
            var employees = await this.employeesSearch.Search(query, token);
            var tasks = employees.Select(
                async x =>
                    {
                        var employee = EmployeeModel.FromMetadata(x.Metadata);
                        var photo = await x.Actor.Ask<GetPhoto.Response>(GetPhoto.Instance, this.timeoutSettings.Timeout, token);
                        employee.Photo = photo.Photo;
                        employee.HoursCredit = 12;
                        employee.VacationDaysLeft = 28;
                        return employee;
                    });

            return await Task.WhenAll(tasks);
        }
    }
}