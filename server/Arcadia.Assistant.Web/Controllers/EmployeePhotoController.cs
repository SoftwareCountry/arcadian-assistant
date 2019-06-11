namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Employees;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/photo/employee")]
    [Authorize]
    public class EmployeePhotoController : Controller
    {
        private readonly IEmployeesRegistry employeesRegistry;

        private readonly ITimeoutSettings timeoutSettings;

        public EmployeePhotoController(IEmployeesRegistry employeesRegistry, ITimeoutSettings timeoutSettings)
        {
            this.employeesRegistry = employeesRegistry;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("{employeeId}")]
        [HttpGet]
        public async Task<IActionResult> GetImage(string employeeId, CancellationToken token)
        {
            var employee = (await this.employeesRegistry.SearchAsync(new EmployeesQuery().WithId(employeeId), token)).FirstOrDefault();
            if (employee == null)
            {
                return this.NotFound();
            }

            var photoContainer = await employee.Actor.Ask<GetPhoto.Response>(GetPhoto.Instance, this.timeoutSettings.Timeout, token);
            if (photoContainer.Photo == null)
            {
                return this.NotFound();
            }

            return this.File(photoContainer.Photo.Bytes, photoContainer.Photo.MimeType);
        }
    }
}