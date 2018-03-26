namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Users;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/feeds")]
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
        [ProducesResponseType(typeof(Message[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMessages([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, CancellationToken token)
        {
            var timeout = this.timeoutSettings.Timeout;

            var employee = await this.userEmployeeSearch.FindOrDefault(this.User, token);
            if (employee == null)
            {
                return this.Ok(new Message[0]);
            }

            var feedsActor = this.actorFactory.ActorSelection(this.pathsBuilder.Get("shared-feeds"));
            var sharedFeeds = 
                await feedsActor.Ask<SharedFeedsActor.GetFeeds.Response>(new SharedFeedsActor.GetFeeds(employee.Metadata.EmployeeId), timeout, token);

            //we should display information from common feeds
            var feeds = new[] { sharedFeeds.System, sharedFeeds.News };

            var responsesTasks = feeds
                .Select(x => x.Ask<GetMessages.Response>(new GetMessages(fromDate, toDate), timeout, token));

            var respones = await Task.WhenAll(responsesTasks);
            var messages = respones.SelectMany(x => x.Messages).OrderByDescending(x => x.DatePosted);

            return this.Ok(messages);
        }
    }
}