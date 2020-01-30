using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace Arcadia.Assistant.PushNotificationsDistributor.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Actors;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IPushNotificationsDistributionActor : IActor
    {
        Task SendPushNotification(PushNotificationMessage message);
    }
}