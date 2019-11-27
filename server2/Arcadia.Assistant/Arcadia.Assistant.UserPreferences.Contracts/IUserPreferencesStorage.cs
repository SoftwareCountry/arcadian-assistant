using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]
namespace Arcadia.Assistant.UserPreferences.Contracts
{
    using Microsoft.ServiceFabric.Actors;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// This interface defines the methods exposed by an actor.
    /// Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IUserPreferencesStorage : IActor
    {
        Task<UserPreferences> Get(CancellationToken cancellationToken);

        Task Set(UserPreferences userPreferences, CancellationToken cancellationToken);
    }
}
