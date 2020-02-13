using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.UserFeeds
{
    using AnniversaryFeed.Contracts;

    using BirthdaysFeed.Contracts;

    using Contracts;

    using Microsoft.ServiceFabric.Data;

    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class UserFeeds : StatefulService, IUserFeeds
    {
        private const string UserFeedsCollectionKey = "user_feeds";

        private readonly Dictionary<string, IFeedService> userFeedsMap;

        public UserFeeds(StatefulServiceContext context, 
            IAnniversaryFeed anniversaryFeed,
            IBirthdaysFeed birthdayFeed)
            : base(context)
        {
            this.userFeedsMap = new Dictionary<string, IFeedService>()
            {
                { AnniversaryFeed.Contracts.Constants.ServiceType, (IFeedService)anniversaryFeed },
                { BirthdaysFeed.Contracts.Constants.ServiceType, (IFeedService)birthdayFeed }
            };
        }

        private TimeSpan Timeout => TimeSpan.FromMinutes(1);

        public async Task<IEnumerable<IFeed>> GetUserFeedList(string employeeId, CancellationToken cancellationToken)
        {
            using var transaction = this.StateManager.CreateTransaction();
            var userFeedsStore = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, IEnumerable<IFeed>>>(transaction, UserFeedsCollectionKey, this.Timeout);
            var userFeeds = await userFeedsStore.TryGetValueAsync(transaction, employeeId, this.Timeout, cancellationToken);
            var userFeedsCollection = userFeeds.HasValue ? userFeeds.Value : CreateUserFeedsCollection(employeeId);
            if (!userFeeds.HasValue)
            {
                await userFeedsStore.TryUpdateAsync(transaction, employeeId, userFeedsCollection, userFeedsCollection, this.Timeout, cancellationToken);
            }

            return userFeedsCollection;
        }

        public async Task<IEnumerable<FeedItem>> GetUserFeeds(string employeeId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var result = new List<FeedItem>();
            using var transaction = this.StateManager.CreateTransaction();
            var userFeedsStore = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, IEnumerable<IFeed>>>(transaction, UserFeedsCollectionKey, this.Timeout);
            var userFeeds = await userFeedsStore.TryGetValueAsync(transaction, employeeId, this.Timeout, cancellationToken);
            var userFeedsCollection = userFeeds.HasValue ? userFeeds.Value : CreateUserFeedsCollection(employeeId);
            foreach (var feed in userFeedsCollection)
            {
                if (feed.Subscribed && this.userFeedsMap.TryGetValue(feed.Type, out var service))
                {
                    var items = await service.GetItems(startDate, endDate, cancellationToken);
                    result.AddRange(items);
                }
            }

            return result.OrderByDescending(x => x.Date);
        }

        public async Task Subscribe(string employeeId, IEnumerable<string> feedIds, CancellationToken cancellationToken)
        {
            using var transaction = this.StateManager.CreateTransaction();
            var userFeedsStore = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, IEnumerable<IFeed>>>(transaction, UserFeedsCollectionKey, this.Timeout);
            var userFeeds = await userFeedsStore.TryGetValueAsync(transaction, employeeId, this.Timeout, cancellationToken);
            var userFeedsCollection = userFeeds.HasValue ? userFeeds.Value : CreateUserFeedsCollection(employeeId);
            userFeedsCollection.ToList().ForEach(f => f.Subscribed = feedIds.Any(i => f.Type == i));
            await userFeedsStore.TryUpdateAsync(transaction, employeeId, userFeedsCollection, userFeedsCollection, this.Timeout, cancellationToken);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return new ServiceReplicaListener[0];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            await this.StateManager.GetOrAddAsync<IReliableDictionary<string, IEnumerable<IFeed>>>(UserFeedsCollectionKey);
        }

        private IEnumerable<IFeed> CreateUserFeedsCollection(string employeeId)
        {
            var availableFeeds = GetUserAvailableFeedTypes(employeeId);
            return availableFeeds.Select(x => new Feed()
            {
                Type = x,
                Name = $"{x} feed"
            });
        }

        private IEnumerable<string> GetUserAvailableFeedTypes(string employeeId)
        {
            return this.userFeedsMap.Keys;
        }
    }
}
