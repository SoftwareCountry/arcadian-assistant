namespace Arcadia.Assistant.DeviceRegistry
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;
    using Contracts.Models;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class DeviceRegistry : StatefulService, IDeviceRegistry
    {
        private readonly ILogger logger;

        public DeviceRegistry(StatefulServiceContext context, ILogger logger)
            : base(context)
        {
            this.logger = logger;
        }

        public async Task RegisterDevice(EmployeeId employeeId, DeviceId deviceId, DeviceType deviceType, CancellationToken cancellationToken)
        {
            var newDeviceItem = new DeviceRegistryEntry
            {
                DeviceId = deviceId,
                DeviceType = deviceType
            };

            await new RegistryOperations(this.StateManager, this.logger)
                .AddDeviceToRegistry(employeeId, newDeviceItem, cancellationToken);
        }

        public async Task RemoveDevice(EmployeeId employeeId, DeviceId deviceId, CancellationToken cancellationToken)
        {
            await new RegistryOperations(this.StateManager, this.logger)
                .RemoveDeviceFromRegistry(employeeId, deviceId, cancellationToken);
        }

        public async Task<IEnumerable<DeviceRegistryEntry>> GetDeviceRegistryByEmployee(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            return await new RegistryOperations(this.StateManager, this.logger)
                .GetDeviceFromRegistryByEmployee(employeeId, cancellationToken);
        }

        public async Task<Dictionary<EmployeeId, IEnumerable<DeviceRegistryEntry>>> GetDeviceRegistryByEmployeeList(IEnumerable<EmployeeId> employeeId, CancellationToken cancellationToken)
        {
            var result = new Dictionary<EmployeeId, IEnumerable<DeviceRegistryEntry>>();
            foreach (var id in employeeId)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new Dictionary<EmployeeId, IEnumerable<DeviceRegistryEntry>>();
                }

                result.Add(id, await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByEmployee(id, cancellationToken));
            }

            return result;
        }

        public async Task<IEnumerable<DeviceRegistryEntry>> GetDeviceRegistryByEmployeeAndType(EmployeeId employeeId, DeviceType deviceType, CancellationToken cancellationToken)
        {
            return (await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByEmployee(employeeId, cancellationToken))
                .Where(x => x.DeviceType == deviceType);
        }
    }
}