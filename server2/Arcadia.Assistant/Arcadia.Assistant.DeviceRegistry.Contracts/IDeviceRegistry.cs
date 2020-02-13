namespace Arcadia.Assistant.DeviceRegistry.Contracts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IDeviceRegistry : IService
    {
        Task RegisterDevice(string employeeId, string deviceId, string deviceType, CancellationToken cancellationToken);

        Task RemoveDevice(string employeeId, string deviceId, CancellationToken cancellationToken);

        Task<IEnumerable<DeviceRegistryItem>> GetDeviceRegistryByEmployee(string employeeId, CancellationToken cancellationToken);

        Task<Dictionary<string, IEnumerable<DeviceRegistryItem>>> GetDeviceRegistryByEmployeeList(IEnumerable<string> employeeId, CancellationToken cancellationToken);

        Task<IEnumerable<DeviceRegistryItem>> GetDeviceRegistryByEmployeeAndType(string employeeId, string deviceType, CancellationToken cancellationToken);
    }
}