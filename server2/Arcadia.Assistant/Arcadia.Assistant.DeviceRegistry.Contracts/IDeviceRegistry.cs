using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.DeviceRegistry.Contracts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDeviceRegistry : IService
    {
        Task RegisterDevice(
            EmployeeId employeeId, DeviceId deviceId, DeviceType deviceType, CancellationToken cancellationToken);

        Task RemoveDevice(EmployeeId employeeId, DeviceId deviceId, CancellationToken cancellationToken);

        Task<Dictionary<EmployeeId, DeviceRegistryEntry[]>> GetDeviceRegistryByDeviceType(
            DeviceType deviceType, CancellationToken cancellationToken);

        Task<DeviceRegistryEntry[]> GetDeviceRegistryByEmployee(
            EmployeeId employeeId, CancellationToken cancellationToken);

        Task<Dictionary<EmployeeId, DeviceRegistryEntry[]>> GetDeviceRegistryByEmployeeList(
            EmployeeId[] employeeId, CancellationToken cancellationToken);

        Task<DeviceRegistryEntry[]> GetDeviceRegistryByEmployeeAndType(
            EmployeeId employeeId, DeviceType deviceType, CancellationToken cancellationToken);
    }
}