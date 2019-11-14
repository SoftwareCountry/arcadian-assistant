using Arcadia.Assistant.CSP;
using Arcadia.Assistant.SharedFeeds.Contracts;
using Arcadia.Assistant.SharedFeeds.Feeds;
using Autofac.Features.OwnedInstances;
using Microsoft.EntityFrameworkCore;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.Assistant.SharedFeeds
{
    public class FeedMessagesStorageHelper
    {
        private readonly IReliableDictionary<DateTime, FeedMessageDictionary> feedsDictionary;
        private readonly IReliableDictionary<DateTime, DateTime> feedUsingDictionary;

        private readonly Dictionary<eFeedType, IFeedRequester> _feedRequesters;
        private readonly Action<string> _logSaver;

        public FeedMessagesStorageHelper(
                IReliableDictionary<DateTime, FeedMessageDictionary> feedsDictionary,
                IReliableDictionary<DateTime, DateTime> feedUsingDictionary,
                Func<Owned<CspEmployeeQuery>> employeeQuery,
                Action<string> logAction = null)
        {
            this.feedsDictionary = feedsDictionary;
            this.feedUsingDictionary = feedUsingDictionary;
            _feedRequesters = new Dictionary<eFeedType, IFeedRequester>()
            {
                [eFeedType.birthday] = new EmployeesBirthdaysFeed(employeeQuery),
                [eFeedType.anniversaries] = new EmployeesAnniversariesFeed(employeeQuery)
            };
            _logSaver = logAction;
        }

        public async Task<ICollection<FeedMessage>> GetFeedsByDatesRange(eFeedType feedType, DateTime fromDate, DateTime toDate, ITransaction transaction, CancellationToken cancellationToken)
        {
            var result = new List<FeedMessage>();
            foreach (var date in GetDateRange(fromDate, toDate))
            {
                var messages = await RequestFeedMessages(transaction, date, cancellationToken);
                if (messages.TryGetValue(feedType, out var list))
                {
                    result.AddRange(list);
                }
            }

            return result;
        }

        #region private methods

        private IEnumerable<DateTime> GetDateRange(DateTime fromDate, DateTime toDate)
        {
            return Enumerable.Range(0, 1 + toDate.Subtract(fromDate).Days)
                .Select(offset => fromDate.AddDays(offset));
        }

        private async Task<Dictionary<eFeedType, ICollection<FeedMessage>>> RequestFeedMessages(ITransaction transaction, DateTime date, CancellationToken cancellationToken)
        {
            var feedMessages = await feedsDictionary.TryGetValueAsync(transaction, date);
            var result = feedMessages.Value;

            if (!feedMessages.HasValue)
            {
                result = new FeedMessageDictionary();
                foreach (var ft in ((eFeedType[])Enum.GetValues(typeof(eFeedType))).Where(x => _feedRequesters.ContainsKey(x)))
                {
                    try
                    {
                        var messages = await _feedRequesters[ft].GetFeedMessages(date, cancellationToken);
                        result.Add(ft, messages);
                    }
                    catch(Exception ex)
                    {
                        _logSaver?.Invoke($"Feed '{ft}' for date {date.ToShortDateString()} request error: {ex.Message}");
                    }
                }

                await feedsDictionary.AddOrUpdateAsync(transaction, date, result, (x, y) => result);
            }

            var today = DateTime.Today;
            await feedUsingDictionary.AddOrUpdateAsync(transaction, date, today, (x, y) => today);

            return result;
        }

        #endregion
    }
}
