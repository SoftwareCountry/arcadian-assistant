namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Authorization;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/feeds")]
    [Authorize(Policies.UserIsEmployee)]
    public class FeedsController : Controller
    {
        private readonly IActorRefFactory actorFactory;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly IUserEmployeeSearch userEmployeeSearch;

        private readonly ITimeoutSettings timeoutSettings;

        public FeedsController(
            ActorPathsBuilder pathsBuilder,
            IActorRefFactory actorFactory,
            ITimeoutSettings timeoutSettings,
            IUserEmployeeSearch userEmployeeSearch)
        {
            this.pathsBuilder = pathsBuilder;
            this.actorFactory = actorFactory;
            this.timeoutSettings = timeoutSettings;
            this.userEmployeeSearch = userEmployeeSearch;
        }

        [Route("messages")]
        [HttpGet]
        [ProducesResponseType(typeof(MessageModel[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMessages([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken token)
        {
            var timeout = this.timeoutSettings.Timeout;
            var today = DateTime.Today;

            var employee = await this.userEmployeeSearch.FindOrDefaultAsync(this.User, token);
            if (employee == null)
            {
                return this.Ok(new Message[0]);
            }

            var responsesTasks = (await this.GetUserFeeds(employee, token))
                .Select(x => x.Ask<GetMessages.Response>(new GetMessages(fromDate ?? today, toDate ?? today), timeout, token));

            var responses = await Task.WhenAll(responsesTasks);
            var messages = responses.SelectMany(x => x.Messages)
                .Distinct(Message.MessageIdComparer)
                .OrderByDescending(x => x.DatePosted)
                .Select(x => new MessageModel(x));

            return this.Ok(messages);
        }

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
    }
}