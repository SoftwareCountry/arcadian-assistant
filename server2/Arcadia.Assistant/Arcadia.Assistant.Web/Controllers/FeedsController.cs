namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Models;

    using UserFeeds.Contracts.Interfaces;
    using UserFeeds.Contracts.Models;

    [Route("api/feeds")]
    [Authorize]
    public class FeedsController : Controller
    {
        private readonly IEmployees employees;
        private readonly ILogger logger;
        private readonly IUserFeeds userFeeds;

        public FeedsController(IEmployees employees, IUserFeeds userFeeds, ILogger logger)
        {
            this.employees = employees;
            this.userFeeds = userFeeds;
            this.logger = logger;
        }

        [Route("messages")]
        [HttpGet]
        [ProducesResponseType(typeof(FeedMessage[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFeedMessages /*GetAllMessages*/(
            [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            var employee = (await this.employees
                    .FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), cancellationToken))
                .FirstOrDefault();
            if (employee == null)
            {
                return this.Ok(new FeedMessage[0]);
            }

            var sharedFeeds =
                await this.userFeeds.GetUserFeeds(employee.EmployeeId, fromDate ?? today, toDate ?? today,
                    cancellationToken);
            var messages = sharedFeeds.Select(x => new FeedMessage(employee.EmployeeId.Value.ToString(), x))
                .Distinct(FeedMessage.MessageIdComparer)
                .OrderByDescending(x => x.DatePosted);

            return this.Ok(messages);
        }

        [Route("feeds")]
        [HttpGet]
        [ProducesResponseType(typeof(FeedModel[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFeeds(CancellationToken cancellationToken)
        {
            var employee = (await this.employees
                    .FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), cancellationToken))
                .FirstOrDefault();
            if (employee == null)
            {
                return this.Ok(new FeedModel[0]);
            }

            var feeds = await this.userFeeds.GetUserFeedList(employee.EmployeeId, cancellationToken);
            var result = feeds.Select(x => new FeedModel
            {
                Type = x.Id.Value,
                Name = x.Name,
                Subscribed = x.IsSubscribed
            });
            return this.Ok(result);
        }

        [Route("subscribe")]
        [HttpPost]
        public async Task<IActionResult> FeedsSubscribe(string[] feedTypes, CancellationToken cancellationToken)
        {
            var employee = (await this.employees
                    .FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), cancellationToken))
                .FirstOrDefault();
            if (employee == null)
            {
                return this.Ok();
            }

            await this.userFeeds.Subscribe(
                employee.EmployeeId,
                feedTypes.Select(x => new FeedId(x))
                    .ToArray(),
                cancellationToken);
            return this.Ok();
        }

        /*
        private async Task<IEnumerable<IActorRef>> GetUserFeeds(EmployeeContainer employee, CancellationToken token)
        {
            var feedsActor = this.actorFactory.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.SharedFeeds));
            var sharedFeedsResponse =
                await feedsActor.Ask<GetFeeds.Response>(new GetFeeds(employee.Metadata.EmployeeId), this.timeoutSettings.Timeout, token);

            var sharedFeeds = sharedFeedsResponse.Feeds.Values;

            var departmentActor = this.actorFactory.ActorSelection(this.pathsBuilder.Get(WellKnownActorPaths.Organization));
            var department =
                await departmentActor.Ask<DepartmentsQuery.Response>(DepartmentsQuery.Create().WithId(employee.Metadata.DepartmentId));

            var departmentFeeds = department.Departments.Select(x => x.Feed);

            return sharedFeeds.Union(departmentFeeds);
        }
        */
    }
}