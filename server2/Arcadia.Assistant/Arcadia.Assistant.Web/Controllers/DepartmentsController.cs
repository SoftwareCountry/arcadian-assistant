using Microsoft.AspNetCore.Mvc;

namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;

    using Organization.Contracts;

    [Route("/api/departments")]
    public class DepartmentsController : Controller
    {
        private IOrganization organization;

        public DepartmentsController(IOrganization organization)
        {
            this.organization = organization;
        }

        [Route("")]
        [HttpGet]
        [ProducesResponseType(typeof(DepartmentMetadata[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> All(CancellationToken token)
        {
            var departments = await this.organization.GetDepartmentsAsync(token);
            return this.Ok(departments.OrderBy(x => x.DepartmentId).ToArray());
        }

        [Route("{departmentId}")]
        [HttpGet]
        [ProducesResponseType(typeof(DepartmentMetadata), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string departmentId, CancellationToken token)
        {
            return this.NotFound();
        }
    }
}