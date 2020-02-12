using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.UserFeeds.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IUserFeeds : IService
    {
        Task<IEnumerable<IFeed>> GetUserFeedList(string employeeId, CancellationToken cancellationToken);

        Task Subscribe(string employeeId, IEnumerable<string> feedIds, CancellationToken cancellationToken);

        Task<IEnumerable<FeedItem>> GetUserFeeds(string employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}