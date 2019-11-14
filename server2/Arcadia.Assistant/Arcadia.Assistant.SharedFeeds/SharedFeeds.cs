using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Arcadia.Assistant.CSP;
using Arcadia.Assistant.SharedFeeds.Contracts;
using Arcadia.Assistant.SharedFeeds.Feeds;
using Autofac.Features.OwnedInstances;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace Arcadia.Assistant.SharedFeeds
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class SharedFeeds : StatefulService, IFeeds
    {
        private const string FeedsDictionaryName = "shared-feeds-cache";
        private const string DatesDictionaryName = "shared-feeds-use-cache";
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;

        public SharedFeeds(StatefulServiceContext context, Func<Owned<CspEmployeeQuery>> employeeQuery)
            : base(context)
        {
            this.employeeQuery = employeeQuery;
        }

        public async Task<ICollection<FeedMessage>> GetAnniversariesFeed(DateTime fromDate, DateTime endDate, CancellationToken cancellationToken)
        {
            try
            {
                return await GetFeed(eFeedType.anniversaries, fromDate, endDate, cancellationToken);
            }
            catch(Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Anniversaries request failed: {0}", ex);
            }

            return new List<FeedMessage>();
        }

        public async Task<ICollection<FeedMessage>> GetBirthdaysFeed(DateTime fromDate, DateTime endDate, CancellationToken cancellationToken)
        {
            try
            {
                return await GetFeed(eFeedType.anniversaries, fromDate, endDate, cancellationToken);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(this.Context, "Birthdays request failed: {0}", ex);
            }

            return new List<FeedMessage>();
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
            var lastUsageDurationDays = TimeSpan.FromDays(3);
            var lastUsageFromCurrentDistanceDays = TimeSpan.FromDays(5);

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var feedDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, FeedMessageDictionary>>(FeedsDictionaryName);
                var usageDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, DateTime>>(DatesDictionaryName);

                var today = DateTime.Today;

                using var tx = this.StateManager.CreateTransaction();

                var handleDates = new List<DateTime>();
                using (var enumerator = (await usageDictionary.CreateEnumerableAsync(tx, EnumerationMode.Ordered)).GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(cancellationToken))
                    {
                        var useDate = enumerator.Current.Value;
                        if ((today - useDate) > lastUsageFromCurrentDistanceDays)
                        {
                            handleDates.Add(enumerator.Current.Key);
                        }
                    }
                }

                foreach (var date in handleDates)
                {
                    try
                    {
                        await feedDictionary.TryRemoveAsync(tx, date);
                        await usageDictionary.TryRemoveAsync(tx, date);
                    }
                    catch(Exception ex)
                    {
                        ServiceEventSource.Current.ServiceMessage(this.Context, "Feed data for {0} remove error: {1}", date, ex);
                    }
                }

                await tx.CommitAsync();

                await Task.Delay(TimeSpan.FromHours(6), cancellationToken);
            }
        }

        private async Task<ICollection<FeedMessage>> GetFeed(eFeedType feedType, DateTime fromDate, DateTime endDate, CancellationToken cancellationToken)
        {
            var feedDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, FeedMessageDictionary>>(FeedsDictionaryName);
            var usageDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<DateTime, DateTime>>(DatesDictionaryName);
            using var tx = this.StateManager.CreateTransaction();
            var requester = new FeedMessagesStorageHelper(feedDictionary, usageDictionary, this.employeeQuery, x => ServiceEventSource.Current.ServiceMessage(this.Context, x));
            var storage = await requester.GetFeedsByDatesRange(feedType, fromDate, endDate, tx, cancellationToken);
            await tx.CommitAsync();
            return storage;
        }
    }
}
