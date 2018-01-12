namespace Arcadia.Assistant.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Server.Interop;

    using Microsoft.AspNetCore.Mvc;

    [Route("api/feeds")]
    public class FeedsController : Controller
    {
        private readonly IActorRefFactory actorFactory;

        private readonly ActorPathsBuilder pathsBuilder;

        public FeedsController(ActorPathsBuilder pathsBuilder, IActorRefFactory actorFactory)
        {
            this.pathsBuilder = pathsBuilder;
            this.actorFactory = actorFactory;
        }

        [Route("messages")]
        [HttpGet]
        [ProducesResponseType(typeof(Message), 200)]
        public async Task<IActionResult> GetAllMessages(CancellationToken token)
        {
            var feedsActor = this.actorFactory.ActorSelection(this.pathsBuilder.Get("shared-feeds"));
            var sharedFeeds = await feedsActor.Ask<SharedFeedsActor.GetFeeds.Response>(SharedFeedsActor.GetFeeds.Instance, TimeSpan.FromSeconds(30), token);

            //we should display information from common feeds
            var feeds = new[] { sharedFeeds.System, sharedFeeds.News };

            var responsesTasks = feeds
                .Select(x => x.Ask<FeedActor.GetMessages.Response>(new FeedActor.GetMessages(), TimeSpan.FromSeconds(30), token));

            var respones = await Task.WhenAll(responsesTasks);
            var messages = respones.SelectMany(x => x.Messages).OrderByDescending(x => x.DatePosted);

            return this.Ok(messages);
        }
    }
}