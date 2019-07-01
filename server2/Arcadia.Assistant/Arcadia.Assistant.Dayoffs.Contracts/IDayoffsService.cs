using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Dayoffs.Contracts
{
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IDayoffsService : IService
    {
        Task<int> GetAsync();
    }
}