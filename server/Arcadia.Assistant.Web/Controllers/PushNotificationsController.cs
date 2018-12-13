namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Arcadia.Assistant.Notifications.Push;
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RegisterDevice([FromBody]string deviceId, CancellationToken cancellationToken)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            await this.pushNotificationsService.RegisterDevice(employee.Metadata.EmployeeId, deviceId, cancellationToken);

            return this.Accepted();
        }

        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveDevice([FromBody] string deviceId, CancellationToken cancellationToken)
        {
            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, cancellationToken);

            var response = await this.pushNotificationsService.RemoveDevice(employee.Metadata.EmployeeId, deviceId, cancellationToken);

            switch (response)
            {
                case RemovePushNotificationsDevice.Success _:
                    return this.Accepted();

                case RemovePushNotificationsDevice.NotFoundError _:
                    return this.NotFound();

                default:
                    return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}