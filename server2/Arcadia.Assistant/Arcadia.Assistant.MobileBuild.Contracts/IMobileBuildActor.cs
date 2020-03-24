using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly:
    FabricTransportActorRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.MobileBuild.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Actors;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface IMobileBuildActor : IActor
    {
        /// <summary>
        ///     Return current stored build data
        /// </summary>
        /// <returns>Stored build data</returns>
        Task<byte[]> GetMobileBuildDataAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Return current stored build version
        /// </summary>
        /// <returns>Stored build version</returns>
        Task<string> GetMobileBuildVersionAsync(CancellationToken cancellationToken);

        /// <summary>
        ///     Update current stored build by data and version
        /// </summary>
        /// <param name="version">New build version</param>
        /// <param name="data">New build data</param>
        Task SetMobileBuildData(string version, byte[] data, CancellationToken cancellationToken);
    }
}