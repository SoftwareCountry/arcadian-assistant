namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Models;

    [Route("api/employees")]
    public class EmployeesController : Controller
    {
        private readonly IEmployees employees;
        private readonly ILogger<EmployeesController> logger;
        private readonly bool sslOffloading = false; //TODO: configured

        public EmployeesController(IEmployees employees, ILogger<EmployeesController> logger)
        {
            this.employees = employees;
            this.logger = logger;
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

            var model = EmployeeModel.FromMetadata(employee);
            this.FillPhotoUrls(new []{ model });

            return this.Ok(model);
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
            var employeesMetadata = await this.employees.FindEmployeesAsync(query, token);
            var employeeModels = employeesMetadata.Select(EmployeeModel.FromMetadata).ToArray();
            this.FillPhotoUrls(employeeModels);

            return this.Ok(employeeModels);
        }

        private void FillPhotoUrls(IEnumerable<EmployeeModel> employeeModels)
        {
            foreach (var employee in employeeModels)
            {
                try
                {
                    var protocol = this.sslOffloading ? "https" : this.Request.Scheme;
                    employee.PhotoUrl = this.Url.Action(nameof(EmployeePhotoController.GetImage), "EmployeePhoto", new { employeeId = employee.EmployeeId }, protocol);
                }
                catch (Exception e)
                {
                    this.logger.LogWarning(e, "Cannot generate PhotoUrl for {0}", employee.EmployeeId);
                }
            }
        }
    }
}