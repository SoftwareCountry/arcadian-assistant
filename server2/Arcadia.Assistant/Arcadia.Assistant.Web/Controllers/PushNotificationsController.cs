namespace Arcadia.Assistant.Web.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;

    using DeviceRegistry.Contracts;
    using DeviceRegistry.Contracts.Models;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Models;

    [Route("/api/push/device")]
    [Authorize]
    public class PushNotificationsController : Controller
    {
        private readonly IDeviceRegistry deviceRegistry;
        private readonly IEmployees employees;
        private readonly ILogger logger;

        public PushNotificationsController(
            IDeviceRegistry deviceRegistry,
            IEmployees employees,
            ILogger<PushNotificationsController> logger)
        {
            this.deviceRegistry = deviceRegistry;
            this.employees = employees;
            this.logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegisterDevice(
            [FromBody] PushNotificationDeviceModel deviceModel, CancellationToken cancellationToken)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest(this.ModelState);
            }

            var deviceType = deviceModel.DeviceType.MobileBuildType();
            var employee = await this.FindOrDefaultAsync(this.User, cancellationToken);

            if (employee != null)
            {
                await this.deviceRegistry.RegisterDevice(
                    employee.EmployeeId,
                    new DeviceId(deviceModel.DevicePushToken),
                    new DeviceType(deviceType),
                    cancellationToken);
            }
            else
            {
                this.logger.LogWarning("{DeviceType} device (id:{DeviceId}) register declined: user not found.",
                    deviceType, deviceModel.DevicePushToken);
            }

            return this.Accepted();
        }

        [Route("{devicePushToken}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> RemoveDevice(string devicePushToken, CancellationToken cancellationToken)
        {
            var employee = await this.FindOrDefaultAsync(this.User, cancellationToken);

            if (employee != null)
            {
                await this.deviceRegistry.RemoveDevice(
                    employee.EmployeeId,
                    new DeviceId(devicePushToken),
                    cancellationToken);
            }
            else
            {
                this.logger.LogWarning("Device (id:{DeviceId}) remove declined: user not found.");
            }

            return this.Accepted();
        }

        private async Task<EmployeeMetadata?> FindOrDefaultAsync(
            ClaimsPrincipal user, CancellationToken cancellationToken)
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