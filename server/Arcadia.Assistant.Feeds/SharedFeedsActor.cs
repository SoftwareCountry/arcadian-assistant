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

        private const string AnniverseriesFeedId = "anniverseries-feed";

        private readonly Dictionary<string, IActorRef> feedsById = new Dictionary<string, IActorRef>();

        public SharedFeedsActor(IActorRef organization)
        {
            var newsFeed = Context.ActorOf(Props.Create(() => new PersistentFeedActor(NewsFeedId)), NewsFeedId);
            var systemFeed = Context.ActorOf(Props.Create(() => new PersistentFeedActor(SystemFeedId)), SystemFeedId);

            var birthdayFeed = Context.ActorOf(EmployeesBirthdaysFeedActor.CreateProps(organization), BirthdaysFeedId);
            var anniverseriesFeed = Context.ActorOf(EmployeesAnniverseriesFeedActor.CreateProps(organization), AnniverseriesFeedId);

            this.feedsById.Add(NewsFeedId, newsFeed);
            this.feedsById.Add(SystemFeedId, systemFeed);
            this.feedsById.Add(BirthdaysFeedId, birthdayFeed);
            this.feedsById.Add(AnniverseriesFeedId, anniverseriesFeed);

            systemFeed.Tell(new PersistentFeedActor.PostMessage(new Message(Guid.NewGuid(), "557", "System is up", "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor blandit velit in sollicitudin. Nulla. ", DateTimeOffset.Now)));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetFeeds _:
                    this.Sender.Tell(new GetFeeds.Response(this.feedsById));
                    break;

                case RequestDepartmentFeed request:
                    var feedId = this.GenerateDepartmentFeedId(request.DepartmentId);
                    this.Sender.Tell();
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private string GenerateDepartmentFeedId(string departmentId)
        {
            return $"department-feed-{Uri.EscapeDataString(departmentId)}";
        }
    }
}