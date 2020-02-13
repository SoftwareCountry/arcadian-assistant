﻿namespace Arcadia.Assistant.DeviceRegistry
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;

    using Models;

    public class RegistryOperations
    {
        private const string DeviceRegistryKey = "device_tokens";
        private const string DeviceOwnersRegistryKey = "device_employee";
        private const int OperationTimeoutMinutes = 1;

        private readonly ILogger logger;
        private readonly IReliableStateManager stateManager;

        public RegistryOperations(IReliableStateManager stateManager, ILogger logger)
        {
            this.stateManager = stateManager;
            this.logger = logger;
        }

        private static TimeSpan OperationTimeout => TimeSpan.FromMinutes(OperationTimeoutMinutes);

        public async Task AddDeviceToRegistry(EmployeeId employeeId, DeviceRegistryItem registryItem, CancellationToken cancellationToken)
        {
            using var transaction = this.stateManager.CreateTransaction();
            var employeeDeviceList = await this.stateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<DeviceRegistryItem>>>(transaction, DeviceRegistryKey);
            var deviceRegistrations = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceRegistrationInfo>>(transaction, DeviceOwnersRegistryKey);

            var registryValue = new List<DeviceRegistryItem>
            {
                registryItem
            };

            var employeDeviceList = await employeeDeviceList.TryGetValueAsync(transaction, employeeId, OperationTimeout, cancellationToken);
            if (employeDeviceList.HasValue)
            {
                if (employeDeviceList.Value.Any(x => x.DeviceId == registryItem.DeviceId))
                {
                    // Nothing to do - device registered
                    this.logger.LogDebug($"Requested device with id={registryItem.DeviceId} already registered for employee.");
                    return;
                }

                registryValue.AddRange(employeDeviceList.Value);
            }

            var deviceOwner = await deviceRegistrations.TryGetValueAsync(transaction, registryItem.DeviceId, OperationTimeout, cancellationToken);
            if (deviceOwner.HasValue && deviceOwner.Value.OwnerId != employeeId)
            {
                // remove specified device id from old owner - the device can be associated with single employee only
                var ownerDeviceList = await employeeDeviceList.TryGetValueAsync(transaction, deviceOwner.Value.OwnerId, OperationTimeout, cancellationToken);
                if (ownerDeviceList.HasValue && ownerDeviceList.Value.Any(x => x.DeviceId == registryItem.DeviceId))
                {
                    var newOwnerDeviceList = ownerDeviceList.Value.Where(x => x.DeviceId != registryItem.DeviceId).ToList();
                    await employeeDeviceList.AddOrUpdateAsync(transaction, deviceOwner.Value.OwnerId, newOwnerDeviceList, (k, o) => newOwnerDeviceList, OperationTimeout, cancellationToken);
                }
            }

            var deviceRegistrationInfo = new DeviceRegistrationInfo(employeeId, registryItem.DeviceType);
            await employeeDeviceList.AddOrUpdateAsync(transaction, employeeId, registryValue, (k, o) => registryValue, OperationTimeout, cancellationToken);
            await deviceRegistrations.AddOrUpdateAsync(transaction, registryItem.DeviceId, deviceRegistrationInfo, (k, o) => deviceRegistrationInfo, OperationTimeout, cancellationToken);

            this.logger.LogDebug($"New device (Id:{registryItem.DeviceId}) registered for employee ({employeeId.ToString()})");
            await transaction.CommitAsync();
        }

        public async Task<IEnumerable<DeviceRegistryItem>> GetDeviceFromRegistryByEmployee(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var transaction = this.stateManager.CreateTransaction();
            var employeeDeviceList = await this.stateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<DeviceRegistryItem>>>(transaction, DeviceRegistryKey);
            var result = await employeeDeviceList.TryGetValueAsync(transaction, employeeId, OperationTimeout, cancellationToken);
            return result.HasValue
                ? result.Value
                : new List<DeviceRegistryItem>();
        }

        public async Task RemoveDeviceFromRegistry(EmployeeId employeeId, string deviceId, CancellationToken cancellationToken)
        {
            using var transaction = this.stateManager.CreateTransaction();
            var employeeDeviceList = await this.stateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<DeviceRegistryItem>>>(transaction, DeviceRegistryKey);
            var deviceRegistrations = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, DeviceRegistrationInfo>>(transaction, DeviceOwnersRegistryKey);

            var employeeDevices = await employeeDeviceList.TryGetValueAsync(transaction, employeeId, OperationTimeout, cancellationToken);
            var deviceRegistration = await deviceRegistrations.TryGetValueAsync(transaction, deviceId, OperationTimeout, cancellationToken);
            if (!employeeDevices.HasValue && !deviceRegistration.HasValue)
            {
                // no one device records found
                this.logger.LogDebug($"Employee have no device with id={deviceId})");
                return;
            }

            // remove from employee list
            if (employeeDevices.HasValue)
            {
                var newEmployeDevice = employeeDevices.Value.Where(x => x.DeviceId != deviceId).ToList();
                if (newEmployeDevice.Count == 0)
                {
                    // remove employee device registry if no one device registered for employee
                    await employeeDeviceList.TryRemoveAsync(transaction, employeeId);
                }
                else
                {
                    // save new device registry for employee
                    await employeeDeviceList.AddOrUpdateAsync(transaction, employeeId, newEmployeDevice, (k, o) => newEmployeDevice, OperationTimeout, cancellationToken);
                }
            }

            // remove device info
            if (deviceRegistration.HasValue)
            {
                await deviceRegistrations.TryRemoveAsync(transaction, deviceId, OperationTimeout, cancellationToken);
            }

            this.logger.LogDebug($"Device (Id:{deviceId}) removed from registry for employee ({employeeId.ToString()})");
            await transaction.CommitAsync();
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