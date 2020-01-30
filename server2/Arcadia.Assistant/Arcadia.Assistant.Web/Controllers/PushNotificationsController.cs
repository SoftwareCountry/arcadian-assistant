namespace Arcadia.Assistant.Web.Controllers
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Models;
    using System.Security.Claims;
    using Employees.Contracts;
    using System.Linq;

    using PushNotificationsDeviceRegistrator.Contracts;
    using PushNotificationsDeviceRegistrator.Contracts.Models;

    [Route("/api/push/device")]
    [Authorize]
    public class PushNotificationsController : Controller
    {
        private readonly IPushNotificationsDeviceRegistrationActorFactory pushNotificationDeviceRegistrationFactory;
        private readonly IEmployees employees;

        public PushNotificationsController(
            IPushNotificationsDeviceRegistrationActorFactory pushNotificationDeviceRegistrationFactory,
            IEmployees employees)
        {
            this.pushNotificationDeviceRegistrationFactory = pushNotificationDeviceRegistrationFactory;
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

            var deviceType = deviceModel.DeviceType.MobileBuildType();
            var employee = await this.FindOrDefaultAsync(this.User, cancellationToken);

            if (employee != null)
            {
                var deviceRegistrationActor = this.pushNotificationDeviceRegistrationFactory.PushNotificationsDeviceRegistrator();
                await deviceRegistrationActor.RegisterDevice(
                    new RegisterPushNotificationsDevice(
                        employee.EmployeeId.ToString(),
                        deviceModel.DevicePushToken,
                        deviceType),
                    cancellationToken);
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
                var deviceRegistrationActor = this.pushNotificationDeviceRegistrationFactory.PushNotificationsDeviceRegistrator();
                await deviceRegistrationActor.RemoveDevice(
                    new RemovePushNotificationsDevice(
                        employee.EmployeeId.ToString(),
                        devicePushToken),
                    cancellationToken);
            }

            return this.Accepted();
        }

        private async Task<EmployeeMetadata?> FindOrDefaultAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
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