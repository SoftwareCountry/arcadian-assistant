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

    using WorkHoursCredit.Contracts;

    [Route("api/employees")]
    [Authorize]
    public class EmployeesController : Controller
    {
        private readonly IEmployees employees;
        private readonly IPermissions permissions;
        private readonly IWorkHoursCredit workHoursCredit;
        private readonly ILogger<EmployeesController> logger;
        private readonly bool sslOffloading = false; //TODO: configured

        public EmployeesController(IEmployees employees, ILogger<EmployeesController> logger, IPermissions permissions, IWorkHoursCredit workHoursCredit)
        {
            this.employees = employees;
            this.logger = logger;
            this.permissions = permissions;
            this.workHoursCredit = workHoursCredit;
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

            var readableEmployees = employeeMetadatas
                .Where(x => allPermissions.GetPermissions(x).HasFlag(EmployeePermissionsEntry.ReadEmployeeInfo))
                .ToList();

            var tasks = readableEmployees
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

                    return Task.FromResult(employee);
                });


            var hours = await this.workHoursCredit.GetAvailableHoursAsync(
                readableEmployees
                    .Where(x => allPermissions.GetPermissions(x).HasFlag(EmployeePermissionsEntry.ReadEmployeeDayoffsCounter))
                    .Select(x => x.EmployeeId)
                    .ToArray(),
                cancellationToken);
            var employeeModels = await Task.WhenAll(tasks);

            foreach (var employeeModel in employeeModels)
            {
                if (hours.TryGetValue(employeeModel.EmployeeId, out var value))
                {
                    employeeModel.HoursCredit = value;
                }
            }

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