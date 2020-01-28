namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;
    using PushNotifications;
    using System.Security.Claims;
    using Employees.Contracts;
    using System.Linq;

    [Route("/api/push/device")]
    [Authorize]
    public class PushNotificationsController : Controller
    {
        private readonly IPushNotificationsService pushNotificationsService;
        private readonly IEmployees employees;

        public PushNotificationsController(
            IPushNotificationsService pushNotificationsService,
            IEmployees employees)
        {
            this.pushNotificationsService = pushNotificationsService;
            this.employees = employees;
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

            var employee = await this.FindOrDefaultAsync(this.User, cancellationToken);

            this.pushNotificationsService.RegisterDevice(
                employee.EmployeeId.ToString(),
                deviceModel.DevicePushToken,
                deviceModel.DeviceType);

            return this.Accepted();
        }

        [Route("{devicePushToken}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RemoveDevice(string devicePushToken, CancellationToken cancellationToken)
        {
            var employee = await this.FindOrDefaultAsync(this.User, cancellationToken);

            this.pushNotificationsService.RemoveDevice(employee.EmployeeId.ToString(), devicePushToken);

            return this.Accepted();
        }

        private async Task<EmployeeMetadata> FindOrDefaultAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

            var query = EmployeesQuery.Create().WithIdentity(user.Identity);
            var employees = await this.employees.FindEmployeesAsync(query, cancellationToken);

            return employees.SingleOrDefault();
        }
    }
}