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
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class DeviceRegistry : StatefulService, IDeviceRegistry
    {
        private readonly ILogger logger;

        public DeviceRegistry(StatefulServiceContext context, ILogger<DeviceRegistry> logger)
            : base(context)
        {
            this.logger = logger;
        }

        public async Task RegisterDevice(
            EmployeeId employeeId, DeviceId deviceId, DeviceType deviceType, CancellationToken cancellationToken)
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

        public async Task<Dictionary<EmployeeId, DeviceRegistryEntry[]>> GetDeviceRegistryByDeviceType(
            DeviceType deviceType, CancellationToken cancellationToken)
        {
            return (await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByDeviceType(deviceType, cancellationToken))
                .ToDictionary(x => x.Key, x => x.Value.ToArray());
        }

        public async Task<DeviceRegistryEntry[]> GetDeviceRegistryByEmployee(
            EmployeeId employeeId, CancellationToken cancellationToken)
        {
            return (await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByEmployee(employeeId, cancellationToken))
                .ToArray();
        }

        public async Task<Dictionary<EmployeeId, DeviceRegistryEntry[]>> GetDeviceRegistryByEmployeeList(
            EmployeeId[] employeeId, CancellationToken cancellationToken)
        {
            var result = new Dictionary<EmployeeId, DeviceRegistryEntry[]>();
            foreach (var id in employeeId)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new Dictionary<EmployeeId, DeviceRegistryEntry[]>();
                }

                result.Add(id, (await new RegistryOperations(this.StateManager, this.logger)
                        .GetDeviceFromRegistryByEmployee(id, cancellationToken))
                    .ToArray());
            }

            return result;
        }

        public async Task<DeviceRegistryEntry[]> GetDeviceRegistryByEmployeeAndType(
            EmployeeId employeeId, DeviceType deviceType, CancellationToken cancellationToken)
        {
            return (await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByEmployee(employeeId, cancellationToken))
                .Where(x => x.DeviceType == deviceType)
                .ToArray();
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await new RegistryOperations(this.StateManager, this.logger)
                .InitializeDictionary(cancellationToken);
        }

        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }
    }
}