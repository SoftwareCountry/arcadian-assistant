namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.SharedFeeds.Contracts;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/feeds")]
    [Authorize]
    public class FeedsController : Controller
    {
        private readonly IFeeds feedsFactory;

        public FeedsController(IFeeds feedsFactory)
        {
            this.feedsFactory = feedsFactory;
        }

        [Route("messages")]
        [HttpGet]
        [ProducesResponseType(typeof(FeedMessage[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMessages([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken token)
        {/*
            var timeout = this.timeoutSettings.Timeout;
            var today = DateTime.Today;

            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);
            if (employee == null)
            {
                return this.Ok(new Message[0]);
            }

            */

            var today = DateTime.Today;
            var sharedFeeds = (await this.feedsFactory.GetAnniversariesFeed(fromDate ?? today, toDate ?? today, token))
                .Union(await this.feedsFactory.GetBirthdaysFeed(fromDate ?? today, toDate ?? today, token));

            var messages = sharedFeeds
                .Distinct(FeedMessage.MessageIdComparer)
                .OrderByDescending(x => x.DatePosted);

            return this.Ok(messages);
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