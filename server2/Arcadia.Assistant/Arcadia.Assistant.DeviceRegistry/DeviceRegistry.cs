namespace Arcadia.Assistant.DeviceRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Castle.Core.Internal;

    using Contracts;
    using Contracts.Models;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Models;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class DeviceRegistry : StatefulService, IDeviceRegistry
    {
        private const string DeviceRegistryKey = "device_tokens";
        private const string DeviceOwnersRegistryKey = "device_employee";
        private const int OperationTimeoutMinutes = 1;
        private readonly ILogger logger;

        public DeviceRegistry(StatefulServiceContext context, ILogger logger)
            : base(context)
        {
            this.logger = logger;
        }

        private static TimeSpan OperationTimeout => TimeSpan.FromMinutes(OperationTimeoutMinutes);

        public async Task RegisterDevice(string employeeId, string deviceId, string deviceType, CancellationToken cancellationToken)
        {
            var newDeviceItem = new DeviceRegistryItem
            {
                DeviceId = deviceId,
                DeviceType = deviceType
            };
            var employeeIdentifier = new EmployeeId(employeeId);

            using var transaction = this.StateManager.CreateTransaction();
            var employeeDeviceList = await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<DeviceRegistryItem>>>(transaction, DeviceRegistryKey);
            var deviceRegistrations = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, DeviceRegistrationInfo>>(transaction, DeviceOwnersRegistryKey);

            var registryValue = new List<DeviceRegistryItem>
            {
                newDeviceItem
            };

            var employeDeviceList = await employeeDeviceList.TryGetValueAsync(transaction, employeeIdentifier, OperationTimeout, cancellationToken);
            if (employeDeviceList.HasValue)
            {
                if (employeDeviceList.Value.Any(x => x.DeviceId == deviceId))
                {
                    // Nothing to do - device registered
                    return;
                }

                registryValue.AddRange(employeDeviceList.Value);
            }

            var deviceOwner = await deviceRegistrations.TryGetValueAsync(transaction, deviceId, OperationTimeout, cancellationToken);
            if (deviceOwner.HasValue && deviceOwner.Value.OwnerId != employeeIdentifier)
            {
                // remove specified device id from old owner - the device can be associated with single employee only
                var ownerDeviceList = await employeeDeviceList.TryGetValueAsync(transaction, deviceOwner.Value.OwnerId, OperationTimeout, cancellationToken);
                if (ownerDeviceList.HasValue && ownerDeviceList.Value.Any(x => x.DeviceId == deviceId))
                {
                    var newOwnerDeviceList = ownerDeviceList.Value.Where(x => x.DeviceId != deviceId).ToList();
                    await employeeDeviceList.AddOrUpdateAsync(transaction, deviceOwner.Value.OwnerId, newOwnerDeviceList, (k, o) => newOwnerDeviceList, OperationTimeout, cancellationToken);
                }
            }

            var deviceRegistrationInfo = new DeviceRegistrationInfo(employeeIdentifier, deviceType);
            await employeeDeviceList.AddOrUpdateAsync(transaction, employeeIdentifier, registryValue, (k, o) => registryValue, OperationTimeout, cancellationToken);
            await deviceRegistrations.AddOrUpdateAsync(transaction, deviceId, deviceRegistrationInfo, (k, o) => deviceRegistrationInfo, OperationTimeout, cancellationToken);

            await transaction.CommitAsync();
        }

        public async Task RemoveDevice(string employeeId, string deviceId, CancellationToken cancellationToken)
        {
            var employeeIdentifier = new EmployeeId(employeeId);

            using var transaction = this.StateManager.CreateTransaction();
            var employeeDeviceList = await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<DeviceRegistryItem>>>(transaction, DeviceRegistryKey);
            var deviceRegistrations = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, DeviceRegistrationInfo>>(transaction, DeviceOwnersRegistryKey);

            var employeeDevices = await employeeDeviceList.TryGetValueAsync(transaction, employeeIdentifier, OperationTimeout, cancellationToken);
            var deviceRegistration = await deviceRegistrations.TryGetValueAsync(transaction, deviceId, OperationTimeout, cancellationToken);
            if (!employeeDevices.HasValue && !deviceRegistration.HasValue)
            {
                // no one device records found
                return;
            }

            // remove from employee list
            if (employeeDevices.HasValue)
            {
                var newEmployeDevice = employeeDevices.Value.Where(x => x.DeviceId != deviceId).ToList();
                if (newEmployeDevice.IsNullOrEmpty())
                {
                    await employeeDeviceList.TryRemoveAsync(transaction, employeeIdentifier);
                }
                else
                {
                    await employeeDeviceList.AddOrUpdateAsync(transaction, employeeIdentifier, newEmployeDevice, (k, o) => newEmployeDevice, OperationTimeout, cancellationToken);
                }
            }

            // remove device info
            if (deviceRegistration.HasValue)
            {
                await deviceRegistrations.TryRemoveAsync(transaction, deviceId, OperationTimeout, cancellationToken);
            }

            await transaction.CommitAsync();
        }

        public async Task<IEnumerable<DeviceRegistryItem>> GetDeviceRegistryByEmployee(string employeeId, CancellationToken cancellationToken)
        {
            return await this.GetEmployeeDeviceRegistry(new EmployeeId(employeeId), cancellationToken);
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

                result.Add(id, await this.GetEmployeeDeviceRegistry(new EmployeeId(id), cancellationToken));
            }

            return result;
        }

        public async Task<IEnumerable<DeviceRegistryItem>> GetDeviceRegistryByEmployeeAndType(string employeeId, string deviceType, CancellationToken cancellationToken)
        {
            return (await this.GetEmployeeDeviceRegistry(new EmployeeId(employeeId), cancellationToken))
                .Where(x => x.DeviceType == deviceType);
        }

        private async Task<IEnumerable<DeviceRegistryItem>> GetEmployeeDeviceRegistry(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var transaction = this.StateManager.CreateTransaction();
            var employeeDeviceList = await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<DeviceRegistryItem>>>(transaction, DeviceRegistryKey);
            var result = await employeeDeviceList.TryGetValueAsync(transaction, employeeId, OperationTimeout, cancellationToken);
            return result.HasValue
                ? result.Value
                : new List<DeviceRegistryItem>();
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

        private sealed class DeviceRegistrationInfo
        {
            public DeviceRegistrationInfo(EmployeeId ownerId, string deviceType)
            {
                this.OwnerId = ownerId;
                this.DeviceType = deviceType;
            }

            public EmployeeId OwnerId { get; }

            public string DeviceType { get; }
        }
    }
}