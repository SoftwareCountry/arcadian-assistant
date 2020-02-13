namespace Arcadia.Assistant.DeviceRegistry
{
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;
    using Contracts.Models;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Models;

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

        public async Task RegisterDevice(string employeeId, string deviceId, string deviceType, CancellationToken cancellationToken)
        {
            var newDeviceItem = new DeviceRegistryItem
            {
                DeviceId = deviceId,
                DeviceType = deviceType
            };
            var employeeIdentifier = new EmployeeId(employeeId);

            await new RegistryOperations(this.StateManager, this.logger)
                .AddDeviceToRegistry(employeeIdentifier, newDeviceItem, cancellationToken);
        }

        public async Task RemoveDevice(string employeeId, string deviceId, CancellationToken cancellationToken)
        {
            var employeeIdentifier = new EmployeeId(employeeId);

            await new RegistryOperations(this.StateManager, this.logger)
                .RemoveDeviceFromRegistry(employeeIdentifier, deviceId, cancellationToken);
        }

        public async Task<IEnumerable<DeviceRegistryItem>> GetDeviceRegistryByEmployee(string employeeId, CancellationToken cancellationToken)
        {
            return await new RegistryOperations(this.StateManager, this.logger)
                .GetDeviceFromRegistryByEmployee(new EmployeeId(employeeId), cancellationToken);
        }

        public async Task<Dictionary<string, IEnumerable<DeviceRegistryItem>>> GetDeviceRegistryByEmployeeList(IEnumerable<string> employeeId, CancellationToken cancellationToken)
        {
            var result = new Dictionary<string, IEnumerable<DeviceRegistryItem>>();
            foreach (var id in employeeId)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return new Dictionary<string, IEnumerable<DeviceRegistryItem>>();
                }

                result.Add(id, await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByEmployee(new EmployeeId(id), cancellationToken));
            }

            return result;
        }

        public async Task<IEnumerable<DeviceRegistryItem>> GetDeviceRegistryByEmployeeAndType(string employeeId, string deviceType, CancellationToken cancellationToken)
        {
            return (await new RegistryOperations(this.StateManager, this.logger)
                    .GetDeviceFromRegistryByEmployee(new EmployeeId(employeeId), cancellationToken))
                .Where(x => x.DeviceType == deviceType);
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle
        ///     client or user requests.
        /// </summary>
        /// <remarks>
        ///     For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }
    }
}