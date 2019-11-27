using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace Arcadia.Assistant.Avatars.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IAvatar : IActor
    {
        Task SetSource(byte[] bytes);

        Task<Photo?> GetPhoto(CancellationToken cancellationToken);
    }
}