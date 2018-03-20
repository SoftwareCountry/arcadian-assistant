namespace Arcadia.Assistant.Feeds
{
    using System;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds.Polls;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class SharedFeedsActor : UntypedActor, ILogReceive
    {

        private readonly IActorRef newsFeed;

        private readonly IActorRef systemFeed;

        private readonly IActorRef queryFeed;

        private const string NewsFeedId = "news-feed";

        private const string SystemFeedId = "system-feed";

        public SharedFeedsActor(IActorRef organization)
        {
            this.newsFeed = Context.ActorOf(Props.Create(() => new FeedActor(NewsFeedId)), NewsFeedId);
            this.systemFeed = Context.ActorOf(Props.Create(() => new FeedActor(SystemFeedId)), SystemFeedId);
            Context.ActorOf(Props.Create(() => new DailyPollsActor(organization, this.newsFeed)), "daily-polls");

            this.queryFeed = Context.ActorOf(Props.Create(() => new PollsActor(organization, this.newsFeed)));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetFeeds _:
                    this.Sender.Tell(new GetFeeds.Response(this.newsFeed, this.systemFeed));
                    break;

                case FeedsQuery _:
                    this.queryFeed.Tell(message as FeedsQuery);
                    this.Self.Tell(new GetFeeds(), this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public sealed class GetFeeds
        {
            public static readonly GetFeeds Instance = new GetFeeds();

            public sealed class Response
            {
                public IActorRef News { get; }

                public IActorRef System { get; }

                public Response(IActorRef news, IActorRef system)
                {
                    this.News = news;
                    this.System = system;
                }
            }
        }
    }
}