namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Calendar.Abstractions.Messages;

    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Security;
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;
    using Arcadia.Assistant.Web.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    [Route("api/employees")]
    [Authorize(Policies.UserIsEmployee)]
    public class EmployeesController : Controller
    {
        private readonly IEmployeesRegistry employeesRegistry;

        private readonly ITimeoutSettings timeoutSettings;

        private readonly IPermissionsLoader permissionsLoader;

        public EmployeesController(IEmployeesRegistry employeesRegistry, ITimeoutSettings timeoutSettings, IPermissionsLoader permissionsLoader)
        {
            this.employeesRegistry = employeesRegistry;
            this.timeoutSettings = timeoutSettings;
            this.permissionsLoader = permissionsLoader;
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

        [Route("{employeeId}/photo")]
        [HttpGet]
        [ProducesResponseType(typeof(PhotoModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPhotoById(string employeeId, CancellationToken token)
        {
            var allPermissions = await this.permissionsLoader.LoadAsync(this.User);
            var employees = await this.employeesRegistry.SearchAsync(new EmployeesQuery().WithId(employeeId), token);
            var validEmployees = employees.Where(x => allPermissions.GetPermissions(x).HasFlag(EmployeePermissionsEntry.ReadEmployeeInfo));

            if (validEmployees.Count() == 0)
            {
                return this.NotFound();
            }
            var photo = await validEmployees.First().Actor.Ask<GetPhoto.Response>(GetPhoto.Instance, this.timeoutSettings.Timeout, token);
            return this.Ok(photo.Photo);
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
            var allPermissions = await this.permissionsLoader.LoadAsync(this.User);
            var employees = await this.employeesRegistry.SearchAsync(query, token);

            var tasks = employees
                .Where(x => allPermissions.GetPermissions(x).HasFlag(EmployeePermissionsEntry.ReadEmployeeInfo))
                .Select(async x =>
                    {
                        var employee = EmployeeModel.FromMetadata(x.Metadata);
                        var employeePermissions = allPermissions.GetPermissions(x);
                        employee.Photo = null;

                        if (!employeePermissions.HasFlag(EmployeePermissionsEntry.ReadEmployeePhone))
                        {
                            employee.MobilePhone = null;
                        }

                        if (employeePermissions.HasFlag(EmployeePermissionsEntry.ReadEmployeeVacationsCounter))
                        {
                            var vacationsCredit = await x.Calendar
                                .VacationsActor
                                .Ask<GetVacationsCredit.Response>(GetVacationsCredit.Instance, this.timeoutSettings.Timeout, token);
                            employee.VacationDaysLeft = vacationsCredit.VacationsCredit;
                        }

                        if (employeePermissions.HasFlag(EmployeePermissionsEntry.ReadEmployeeDayoffsCounter))
                        {
                            var workhoursCredit = await x.Calendar
                                .WorkHoursActor
                                .Ask<GetWorkHoursCredit.Response>(GetWorkHoursCredit.Instance, this.timeoutSettings.Timeout, token);
                            employee.HoursCredit = workhoursCredit.WorkHoursCredit;
                        }
                        return employee;
                    });

            return await Task.WhenAll(tasks);
        }
    }
}