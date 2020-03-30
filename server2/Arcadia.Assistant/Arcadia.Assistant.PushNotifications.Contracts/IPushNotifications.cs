using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.PushNotifications.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    using DeviceRegistry.Contracts.Models;

    using Microsoft.ServiceFabric.Services.Remoting;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IPushNotifications : IService
    {
        Task SendPushNotification(
            DeviceRegistryEntry[] deviceTokens,
            PushNotificationContent notificationContent,
            CancellationToken cancellationToken);
    }
}