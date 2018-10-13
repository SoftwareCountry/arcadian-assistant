namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds.Employees;
    using Arcadia.Assistant.Feeds.Messages;

    public class SharedFeedsActor : UntypedActor, ILogReceive
    {
        private const string NewsFeedId = "news-feed";

        private const string SystemFeedId = "system-feed";

        private const string BirthdaysFeedId = "birthdays-feed";

        private const string AnniversariesFeedId = "anniversaries-feed";

        private readonly Dictionary<string, IActorRef> feedsById = new Dictionary<string, IActorRef>();

        public SharedFeedsActor(IActorRef organization)
        {
            var newsFeed = Context.ActorOf(Props.Create(() => new PersistentFeedActor(NewsFeedId)), NewsFeedId);
            var systemFeed = Context.ActorOf(Props.Create(() => new PersistentFeedActor(SystemFeedId)), SystemFeedId);

            var birthdayFeed = Context.ActorOf(EmployeesBirthdaysFeedActor.CreateProps(organization), BirthdaysFeedId);
            var anniversariesFeed = Context.ActorOf(EmployeesAnniversariesFeedActor.CreateProps(organization), AnniversariesFeedId);

            this.feedsById.Add(NewsFeedId, newsFeed);
            this.feedsById.Add(SystemFeedId, systemFeed);
            this.feedsById.Add(BirthdaysFeedId, birthdayFeed);
            this.feedsById.Add(AnniversariesFeedId, anniversariesFeed);

            // systemFeed.Tell(new PostMessage(new Message(Guid.NewGuid().ToString(), null, "System is up", "System has started", DateTime.UtcNow)));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetFeeds _:
                    this.Sender.Tell(new GetFeeds.Response(this.feedsById));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}