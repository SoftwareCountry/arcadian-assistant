namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Feeds.Polls;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class PollsActor : UntypedPersistentActor, ILogReceive
    {
        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IEnumerable<IActorRef> polls;

        public override string PersistenceId { get; } = "feed-by-dates";

        public PollsActor(IActorRef organizationActor, IActorRef feedActor)
        {
            this.polls = new List<IActorRef>()
                {
                    Context.ActorOf(Props.Create(() => new EmployeeAnniverseryPoll(organizationActor, feedActor))),
                    Context.ActorOf(Props.Create(() => new EmployeeBirthdaysPoll(organizationActor, feedActor)))
                };
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case FeedsQuery _:
                    var query = message as FeedsQuery;
                    for (var date = query.From; date <= query.To; date = date.AddDays(1))
                    {
                        this.Self.Tell(new PerformPoll(date));
                    }

                    break;

                case PerformPoll cmd:
                    this.logger.Debug($"Running a dates check for {cmd.Date}");
                    foreach (var poll in this.polls)
                    {
                        poll.Tell(cmd);
                    }

                    break;
            }
        }

        protected override void OnRecover(object message)
        {
        }
    }
}