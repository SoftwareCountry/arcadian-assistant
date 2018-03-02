namespace Arcadia.Assistant.Feeds
{
    using System;

    using Akka.Actor;

    public class SharedFeedsActor : UntypedActor, ILogReceive
    {
        private readonly IActorRef newsFeed;

        private readonly IActorRef systemFeed;

        private const string NewsFeedId = "news-feed";

        private const string SystemFeedId = "system-feed";

        public SharedFeedsActor(IActorRef organization)
        {
            this.newsFeed = Context.ActorOf(Props.Create(() => new FeedActor(NewsFeedId)), NewsFeedId);
            this.systemFeed = Context.ActorOf(Props.Create(() => new FeedActor(SystemFeedId)), SystemFeedId);
            Context.ActorOf(Props.Create(() => new DailyPollsActor(organization, this.newsFeed)), "daily-polls");

            this.systemFeed.Tell(new FeedActor.PostMessage(new Message(Guid.NewGuid(), "557", "System is up", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor blandit velit in sollicitudin. Nulla. ", DateTimeOffset.Now)));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetFeeds _:
                    this.Sender.Tell(new GetFeeds.Response(this.newsFeed, this.systemFeed));
                    break;
                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public sealed class GetFeeds
        {
            public static GetFeeds Instance = new GetFeeds();

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