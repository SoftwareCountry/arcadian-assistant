namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.PushNotifications;
    using Arcadia.Assistant.Web.Users;

    [Route("/api/push/device")]
    [Authorize(Policies.UserIsEmployee)]
    public class PushNotificationsController : Controller
    {
        private readonly IPushNotificationsService pushNotificationsService;
        private readonly IUserEmployeeSearch userEmployeeSearch;

        public PushNotificationsController(
            IPushNotificationsService pushNotificationsService,
            IUserEmployeeSearch userEmployeeSearch)
        {
            this.pushNotificationsService = pushNotificationsService;
            this.userEmployeeSearch = userEmployeeSearch;
        }

        [Route("{devicePushToken}")]
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RegisterDevice(string devicePushToken, CancellationToken cancellationToken)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            this.pushNotificationsService.RegisterDevice(employee.Metadata.EmployeeId, devicePushToken);

            return this.Accepted();
        }

        [Route("{devicePushToken}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveDevice(string devicePushToken, CancellationToken cancellationToken)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            this.pushNotificationsService.RemoveDevice(employee.Metadata.EmployeeId, devicePushToken);

            return this.Accepted();
        }
    }
}