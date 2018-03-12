namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence;

    using Arcadia.Assistant.Feeds.Polls;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class DailyPollsActor : UntypedPersistentActor, ILogReceive
    {
        private DateTime? lastPollDate = null;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly IEnumerable<IActorRef> polls;

        public override string PersistenceId { get; } = "feed-important-dates";

        public DailyPollsActor(IActorRef organizationActor, IActorRef feedActor)
        {
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1), this.Self, new CheckIfDatesPollIsRequired(), this.Self);

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
                case CheckIfDatesPollIsRequired _:
                    var currentDate = DateTimeOffset.UtcNow.Date;
                    if (!this.lastPollDate.HasValue || (currentDate > this.lastPollDate))
                    {
                        if (this.lastPollDate.HasValue)
                        {
                            for (var date = this.lastPollDate.Value.AddDays(1); date <= currentDate; date = date.AddDays(1))
                            {
                                this.Self.Tell(new PerformPoll(date));
                            }
                        }
                        else
                        {
                            this.logger.Info("Dates check has never been run before, running for the first time...");
                            this.Self.Tell(new PerformPoll(currentDate));
                        }

                        this.Persist(new DailyPollWasRunEvent(currentDate), this.OnCheckWasRunEvent);
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
            switch (message)
            {
                case DailyPollWasRunEvent e:
                    this.OnCheckWasRunEvent(e);
                    break;
            }
        }

        private void OnCheckWasRunEvent(DailyPollWasRunEvent e)
        {
            if (!this.lastPollDate.HasValue || (e.Date > this.lastPollDate.Value))
            {
                this.lastPollDate = e.Date;
            }
        }

        private class CheckIfDatesPollIsRequired
        {
        }
    }
}