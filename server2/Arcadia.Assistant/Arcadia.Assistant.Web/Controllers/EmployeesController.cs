namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Models;

    using Permissions.Contracts;

    [Route("api/employees")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployees employees;
        private readonly IPermissions permissions;
        private readonly ILogger<EmployeesController> logger;
        private readonly bool sslOffloading = false; //TODO: configured

        public EmployeesController(IEmployees employees, ILogger<EmployeesController> logger, IPermissions permissions)
        {
            this.employees = employees;
            this.logger = logger;
            this.permissions = permissions;
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

            var model = (await this.ProcessEmployeesAsync(new[] { employee }, token)).FirstOrDefault();
            if (model == null)
            {
                return this.NotFound();
            }

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
            var employeeModels = await this.ProcessEmployeesAsync(employeesMetadata, token);

            return this.Ok(employeeModels);
        }

        private async Task<EmployeeModel[]> ProcessEmployeesAsync(IEnumerable<EmployeeMetadata> employeeMetadatas, CancellationToken cancellationToken)
        {
            var allPermissions = await this.permissions.GetPermissionsAsync(this.User.Identity.Name, cancellationToken);
            var tasks = employeeMetadatas
                .Where(x => allPermissions.GetPermissions(x).HasFlag(EmployeePermissionsEntry.ReadEmployeeInfo))
                .Select(x =>
                {
                    var employee = EmployeeModel.FromMetadata(x);
                    var employeePermissions = allPermissions.GetPermissions(x);

                    if (!employeePermissions.HasFlag(EmployeePermissionsEntry.ReadEmployeePhone))
                    {
                        employee.MobilePhone = null;
                    }

                    if (employeePermissions.HasFlag(EmployeePermissionsEntry.ReadEmployeeVacationsCounter))
                    {
                        //TODO: vacations count
                        employee.VacationDaysLeft = 5;
                    }

                    if (employeePermissions.HasFlag(EmployeePermissionsEntry.ReadEmployeeDayoffsCounter))
                    {
                        //TODO: dayoffs count
                        employee.HoursCredit = 3;
                    }

                    return Task.FromResult(employee);
                });

            var employeeModels = await Task.WhenAll(tasks);
            this.FillPhotoUrls(employeeModels);
            return employeeModels;
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