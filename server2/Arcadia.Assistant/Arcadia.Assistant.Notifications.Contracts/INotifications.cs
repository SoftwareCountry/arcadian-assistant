using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Notifications.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface INotifications : IService
    {
        Task Send(
            EmployeeId[] employeeIds,
            NotificationMessage notificationMessage,
            CancellationToken cancellationToken);
    }
}