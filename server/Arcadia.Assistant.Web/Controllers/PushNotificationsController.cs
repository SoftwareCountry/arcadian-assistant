namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Models;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterDevice([FromBody] PushNotificationDeviceModel deviceModel, CancellationToken cancellationToken)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            this.pushNotificationsService.RegisterDevice(
                employee.Metadata.EmployeeId,
                deviceModel.DevicePushToken,
                deviceModel.DeviceType);

            return this.Accepted();
        }

        [Route("{devicePushToken}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RemoveDevice(string devicePushToken, CancellationToken cancellationToken)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            this.pushNotificationsService.RemoveDevice(employee.Metadata.EmployeeId, devicePushToken);

            return this.Accepted();
        }
    }
}