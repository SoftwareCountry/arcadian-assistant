using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]
namespace Arcadia.Assistant.SharedFeeds.Contracts
{
    public interface IFeeds : IService
    {
        Task<ICollection<FeedMessage>> GetAnniversariesFeed(DateTime fromDate, DateTime endDate, CancellationToken cancellationToken);

        Task<ICollection<FeedMessage>> GetBirthdaysFeed(DateTime fromDate, DateTime endDate, CancellationToken cancellationToken);
    }
}