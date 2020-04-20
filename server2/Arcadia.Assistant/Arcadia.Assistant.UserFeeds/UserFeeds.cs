namespace Arcadia.Assistant.UserFeeds
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using AnniversaryFeed.Contracts;

    using BirthdaysFeed.Contracts;

    using Contracts;
    using Contracts.Interfaces;
    using Contracts.Models;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class UserFeeds : StatefulService, IUserFeeds
    {
        private const string UserFeedsCollectionKey = "user_feeds";

        private readonly ILogger logger;
        private readonly Dictionary<FeedId, IFeedService> userFeedsMap;

        public UserFeeds(
            StatefulServiceContext context,
            IAnniversaryFeed anniversaryFeed,
            IBirthdaysFeed birthdayFeed,
            ILogger<UserFeeds> logger)
            : base(context)
        {
            this.logger = logger;
            this.userFeedsMap = new List<IFeedService>
            {
                anniversaryFeed, birthdayFeed
            }.ToDictionary(x => new FeedId(x.ServiceType), x => x);
        }

        private TimeSpan Timeout => TimeSpan.FromMinutes(1);

        public async Task<Feed[]> GetUserFeedList(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var transaction = this.StateManager.CreateTransaction();
            var userFeedsStore =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<Feed>>>(transaction,
                    UserFeedsCollectionKey, this.Timeout);
            var userFeeds =
                await userFeedsStore.TryGetValueAsync(transaction, employeeId, this.Timeout, cancellationToken);
            var userFeedsCollection = userFeeds.HasValue ? userFeeds.Value : this.CreateUserFeedsCollection(employeeId);
            if (!userFeeds.HasValue)
            {
                await userFeedsStore.TryUpdateAsync(transaction, employeeId, userFeedsCollection, userFeedsCollection,
                    this.Timeout, cancellationToken);
            }

            return userFeedsCollection
                .ToArray();
        }

        public async Task<FeedItem[]> GetUserFeeds(
            EmployeeId employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var result = new List<FeedItem>();
            using var transaction = this.StateManager.CreateTransaction();
            var userFeedsStore =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<Feed>>>(transaction,
                    UserFeedsCollectionKey, this.Timeout);
            var userFeeds =
                await userFeedsStore.TryGetValueAsync(transaction, employeeId, this.Timeout, cancellationToken);
            var userFeedsCollection = userFeeds.HasValue ? userFeeds.Value : this.CreateUserFeedsCollection(employeeId);
            foreach (var feed in userFeedsCollection)
            {
                if (feed.FeedSubscription && this.userFeedsMap.TryGetValue(feed.Id, out var service))
                {
                    var items = await service.GetItems(startDate, endDate, cancellationToken);
                    result.AddRange(items);
                }
            }

            return result
                .OrderByDescending(x => x.Date)
                .ToArray();
        }

        public async Task Subscribe(EmployeeId employeeId, FeedId[] feedIds, CancellationToken cancellationToken)
        {
            using var transaction = this.StateManager.CreateTransaction();
            var userFeedsStore =
                await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<Feed>>>(transaction,
                    UserFeedsCollectionKey, this.Timeout);
            var userFeeds =
                await userFeedsStore.TryGetValueAsync(transaction, employeeId, this.Timeout, cancellationToken);
            var userFeedsCollection = userFeeds.HasValue ? userFeeds.Value : this.CreateUserFeedsCollection(employeeId);
            userFeedsCollection.ToList().ForEach(f => f.FeedSubscription = feedIds.Any(i => f.Id == i));
            await userFeedsStore.TryUpdateAsync(transaction, employeeId, userFeedsCollection, userFeedsCollection,
                this.Timeout, cancellationToken);
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle
        ///     client or user requests.
        /// </summary>
        /// <remarks>
        ///     For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        ///     This is the main entry point for your service replica.
        ///     This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await this.StateManager.GetOrAddAsync<IReliableDictionary<EmployeeId, List<Feed>>>(
                UserFeedsCollectionKey);
        }

        private List<Feed> CreateUserFeedsCollection(EmployeeId employeeId)
        {
            var availableFeeds = this.GetUserAvailableFeedTypes(employeeId);
            return availableFeeds.Select(x => new Feed
                {
                    Id = x,
                    Name = $"{x} feed"
                })
                .ToList();
        }

        private IReadOnlyCollection<FeedId> GetUserAvailableFeedTypes(EmployeeId employeeId)
        {
            return this.userFeedsMap.Keys;
        }
    }
}