using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Permissions.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPermissions : IService
    {
        Task<UserPermissionsCollection> GetPermissionsAsync(UserIdentity identity, CancellationToken cancellationToken);
    }
}