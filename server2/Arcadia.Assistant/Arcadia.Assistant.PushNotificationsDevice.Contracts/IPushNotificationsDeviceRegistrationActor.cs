using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace Arcadia.Assistant.PushNotificationsDeviceRegistrator.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Actors;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IPushNotificationsDeviceRegistrationActor : IActor
    {
        Task RegisterDevice(RegisterPushNotificationsDevice message, CancellationToken cancellationToken);

        Task RemoveDevice(RemovePushNotificationsDevice message, CancellationToken cancellationToken);
    }
}