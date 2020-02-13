namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Arcadia.Assistant.Employees.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    using Models;

    using UserFeeds.Contracts;

    [Route("api/feeds")]
    [Authorize]
    public class FeedsController : Controller
    {
        private readonly IEmployees employees;
        private readonly IUserFeeds userFeeds;
        private readonly ILogger logger;

        public FeedsController(IEmployees employees, IUserFeeds userFeeds, ILogger logger)
        {
            this.employees = employees;
            this.userFeeds = userFeeds;
            this.logger = logger;
        }

        [Route("messages")]
        [HttpGet]
        [ProducesResponseType(typeof(FeedMessage[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFeedMessages/*GetAllMessages*/([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken cancellationToken)
        {
            var today = DateTime.Today;

            var employee = (await this.employees
                    .FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(this.User.Identity), cancellationToken))
                .FirstOrDefault();
            if (employee == null)
            {
                return this.Ok(new FeedMessage[0]);
            }

            var employeeId = employee.EmployeeId.ToString();
            var sharedFeeds = await userFeeds.GetUserFeeds(employeeId, fromDate ?? today, toDate ?? today, cancellationToken);
            var messages = sharedFeeds.Select(x => new FeedMessage(employeeId, x))
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

            var employeeId = employee.EmployeeId.ToString();
            var feeds = await this.userFeeds.GetUserFeedList(employeeId, cancellationToken);
            var result = feeds.Select(x => new FeedModel()
            {
                Type = x.Type,
                Name = x.Name,
                Subscribed = x.Subscribed
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

            var employeeId = employee.EmployeeId.ToString();
            await this.userFeeds.Subscribe(employeeId, feedTypes, cancellationToken);
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