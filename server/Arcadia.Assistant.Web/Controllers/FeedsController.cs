namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/feeds")]
    public class FeedsController : Controller
    {
        private readonly IActorRefFactory actorFactory;

        private readonly ActorPathsBuilder pathsBuilder;

        private readonly ITimeoutSettings timeoutSettings;

        public FeedsController(ActorPathsBuilder pathsBuilder, IActorRefFactory actorFactory, ITimeoutSettings timeoutSettings)
        {
            this.pathsBuilder = pathsBuilder;
            this.actorFactory = actorFactory;
            this.timeoutSettings = timeoutSettings;
        }

        [Route("messages")]
        [HttpGet]
        [ProducesResponseType(typeof(Message[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllMessages([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken token)
        {
            var timeout = this.timeoutSettings.Timeout;

            var feedsActor = this.actorFactory.ActorSelection(this.pathsBuilder.Get("shared-feeds"));
            SharedFeedsActor.GetFeeds.Response sharedFeeds = null;

            var query = new FeedsQuery();
            if (from.HasValue && to.HasValue) {
                query = query.InDateRange(from.Value, to.Value);
                sharedFeeds = await feedsActor.Ask<SharedFeedsActor.GetFeeds.Response>(query, timeout, token);
            } else {
                sharedFeeds = await feedsActor.Ask<SharedFeedsActor.GetFeeds.Response>(SharedFeedsActor.GetFeeds.Instance, timeout, token);
            }

            //we should display information from common feeds
            var feeds = new[] { sharedFeeds.System, sharedFeeds.News };

            var responsesTasks = feeds
                .Select(x => x.Ask<FeedActor.GetMessages.Response>(new FeedActor.GetMessages(), timeout, token));

            var respones = await Task.WhenAll(responsesTasks);
            var messages = respones.SelectMany(x => x.Messages).OrderByDescending(x => x.DatePosted);

            return this.Ok(messages);
        }
    }
}