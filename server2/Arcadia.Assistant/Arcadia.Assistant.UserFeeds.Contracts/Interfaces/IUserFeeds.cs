using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.UserFeeds.Contracts.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    using Models;

    public interface IUserFeeds : IService
    {
        Task<IFeed[]> GetUserFeedList(EmployeeId employeeId, CancellationToken cancellationToken);

        Task Subscribe(EmployeeId employeeId, FeedId[] feedIds, CancellationToken cancellationToken);

        Task<FeedItem[]> GetUserFeeds(
            EmployeeId employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
    }
}