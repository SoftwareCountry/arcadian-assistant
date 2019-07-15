using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.SickLeaves.Contracts
{
    using Microsoft.ServiceFabric.Services.Remoting;

    public interface ISickLeaves : IService
    {
        
    }
}