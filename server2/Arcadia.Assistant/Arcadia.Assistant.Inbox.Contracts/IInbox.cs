using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Inbox.Contracts
{
    using Microsoft.ServiceFabric.Services.Remoting;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IInbox : IService
    {
        Task<Email[]> GetEmailsAsync(EmailSearchQuery query, CancellationToken cancellationToken);
    }
}