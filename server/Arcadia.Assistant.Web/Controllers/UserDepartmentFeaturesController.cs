namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.DepartmentFeatures;
    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Policies.UserIsEmployee)]
    [Route("/api/user-department-features")]
    public class UserDepartmentFeaturesController : Controller
    {
        private readonly IDepartmentFeaturesService departmentFeaturesService;
        private readonly IUserEmployeeSearch userEmployeeSearch;

        public UserDepartmentFeaturesController(
            IDepartmentFeaturesService departmentFeaturesService,
            IUserEmployeeSearch userEmployeeSearch)
        {
            this.departmentFeaturesService = departmentFeaturesService;
            this.userEmployeeSearch = userEmployeeSearch;
        }

        [HttpGet]
        [ProducesResponseType(typeof(DepartmentFeaturesModel), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserDepartmentFeatures(CancellationToken cancellationToken)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            var departmentFeatures = await this.departmentFeaturesService.GetDepartmentFeatures(
                employee.Metadata.DepartmentId,
                cancellationToken);

            return this.Ok(departmentFeatures);
        }
    }
}