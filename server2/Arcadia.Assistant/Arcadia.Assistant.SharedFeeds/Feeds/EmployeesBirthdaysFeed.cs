using Arcadia.Assistant.CSP;
using Arcadia.Assistant.CSP.Model;
using Arcadia.Assistant.SharedFeeds.Contracts;
using Autofac.Features.OwnedInstances;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Arcadia.Assistant.SharedFeeds.Feeds
{
    public class EmployeesBirthdaysFeed : IFeedRequester
    {
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;

        public EmployeesBirthdaysFeed(Func<Owned<CspEmployeeQuery>> employeeQuery)
        {
            this.employeeQuery = employeeQuery;
        }

        async Task<ICollection<FeedMessage>> IFeedRequester.GetFeedMessages(DateTime date, CancellationToken cancellation)
        {
            using (var query = this.employeeQuery())
            {
                return (await query.Value.Get()
                    .Where(x => x.IsWorking && !x.IsDelete)
                    .Where(x => x.Birthday.HasValue && !x.FiringDate.HasValue)
                    .Where(x => x.Birthday.Value.Month == date.Month && x.Birthday.Value.Day == date.Day)
                    .ToListAsync(cancellation))
                    .Select(ConvertFeedMessage).ToArray();
            }
        }

        private FeedMessage ConvertFeedMessage(Employee employee)
        {
            var employeeid = employee.Id.ToString();
            var date = employee.Birthday.Value;
            var pronoun = employee.Gender == "F" ? "her" : "his";
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var text = $"{title} celebrates {pronoun} birthday on {date.ToString("MMMM dd")}!";
            return new FeedMessage($"employee-birthday-{employeeid}-at-{date}", employeeid, title, text, date);
        }
    }
}
