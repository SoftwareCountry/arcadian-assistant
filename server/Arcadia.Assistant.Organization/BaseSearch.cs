namespace Arcadia.Assistant.Organization
{
    using System.Collections.Generic;

    using Akka.Actor;

    public abstract class BaseSearch<TFinding, TTargetQuery, TTargetQueryResponse, TSearchResultMessage> : UntypedActor
        where TFinding : class
        where TSearchResultMessage: class
    {
        private readonly HashSet<IActorRef> requesters;

        private readonly HashSet<IActorRef> actorsToReply = new HashSet<IActorRef>();

        private readonly HashSet<TFinding> findings = new HashSet<TFinding>();

        protected BaseSearch(IEnumerable<IActorRef> requesters, IActorRef target)
        {
            this.requesters = new HashSet<IActorRef>(requesters);

            this.actorsToReply.Add(target);

            if (this.requesters.Count == 0)
            {
                this.Self.Tell(PoisonPill.Instance);
            }

            this.Self.Tell(StartSearch.Instance);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartSearch _:
                    foreach (var actorRef in this.actorsToReply)
                    {
                        actorRef.Tell(this.GetTargetQuery());
                    }
                    this.Become(this.GatheringInformation);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void GatheringInformation(object message)
        {
            switch (message)
            {
                case TTargetQueryResponse result:

                    foreach (var finding in this.ExtractFindings(result))
                    {
                        this.findings.Add(finding);
                    }

                    foreach (var actor in this.GetAdditionalTargets(result))
                    {
                        this.actorsToReply.Add(actor);
                        actor.Tell(this.GetTargetQuery());
                    }

                    this.actorsToReply.Remove(this.Sender);

                    if (this.actorsToReply.Count == 0)
                    {
                        this.Self.Tell(SearchFinished.Instance);
                        this.Become(this.SearchCompleted);
                    }

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void SearchCompleted(object message)
        {
            switch (message)
            {
                case SearchFinished _:
                    //reply back to requester

                    foreach (var requester in this.requesters)
                    {
                        requester.Tell(this.FindingsToResponse(this.findings));
                    }

                    this.Self.Tell(PoisonPill.Instance);

                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract IEnumerable<TFinding> ExtractFindings(TTargetQueryResponse targetResponse);

        protected abstract IEnumerable<IActorRef> GetAdditionalTargets(TTargetQueryResponse targetResponse);

        protected abstract TTargetQuery GetTargetQuery();

        protected abstract TSearchResultMessage FindingsToResponse(IReadOnlyCollection<TFinding> findings);

        private class StartSearch
        {
            public static readonly StartSearch Instance = new StartSearch();
        }

        private class SearchFinished
        {
            public static readonly SearchFinished Instance = new SearchFinished();
        }
    }
}