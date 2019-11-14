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
    public class EmployeesAnniversariesFeed : IFeedRequester
    {
        private readonly Func<Owned<CspEmployeeQuery>> employeeQuery;

        public EmployeesAnniversariesFeed(Func<Owned<CspEmployeeQuery>> employeeQuery)
        {
            this.employeeQuery = employeeQuery;
        }

        async Task<ICollection<FeedMessage>> IFeedRequester.GetFeedMessages(DateTime date, CancellationToken cancellation)
        {
            using (var query = this.employeeQuery())
            {
                return (await query.Value.Get()
                    .Where(x => x.IsWorking) // fired employes also
                    .Where(x => x.HiringDate.Month == date.Month && x.HiringDate.Day == date.Day)
                    .ToListAsync(cancellation))
                    .Select(ConvertFeedMessage).ToArray();
            }
        }

        #region private methods

        private FeedMessage ConvertFeedMessage(Employee employee)
        {
            var title = $"{employee.LastName} {employee.FirstName}".Trim();
            var date = DateTime.UtcNow;
            var employeeId = employee.Id.ToString();
            var msg = $"Congratulations with Anniversary! {YearsServedAt(employee, date)} years served!";
            return new FeedMessage($"employee-anniversary-{employeeId}-at-{date}", employeeId, title, msg, date);
        }

        public int? YearsServedAt(Employee employee, DateTime date)
        {
            DateTime toDate;
            if (employee.FiringDate == null)
            {
                toDate = date;
            }
            else
            {
                toDate = date > employee.FiringDate ? employee.FiringDate.Value : date;
            }

            return CalculateYearsFromDate(employee.HiringDate, toDate);
        }

        private static int? CalculateYearsFromDate(DateTime? fromDate, DateTime? toDate = null)
        {
            if (fromDate == null)
            {
                return null;
            }

            if (toDate == null)
            {
                toDate = DateTime.Now;
            }

            var years = toDate.Value.Year - fromDate.Value.Year;

            if ((fromDate.Value.Month > toDate.Value.Month) || ((fromDate.Value.Month == toDate.Value.Month) && (fromDate.Value.Day > toDate.Value.Day)))
            {
                years = years - 1;
            }

            return years;
        }

        #endregion
    }
}
