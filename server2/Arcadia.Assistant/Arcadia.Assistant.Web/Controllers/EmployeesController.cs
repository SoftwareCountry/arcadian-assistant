namespace Arcadia.Assistant.Web.Controllers
{
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
    }
}