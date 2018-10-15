namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Feeds.Messages;

    public class AggregateMessagesActor : UntypedActor, ILogReceive
    {
        private readonly GetMessages request;

        private readonly IActorRef requester;

        private readonly HashSet<IActorRef> actorsToRespond;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        private readonly List<Message> messages = new List<Message>();

        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        public static Props GetProps(IEnumerable<IActorRef> actors, GetMessages request, IActorRef requester) => 
            Props.Create(() => new AggregateMessagesActor(actors, request, requester));

        public AggregateMessagesActor(IEnumerable<IActorRef> actors, GetMessages request, IActorRef requester)
        {
            this.request = request;
            this.requester = requester;
            this.actorsToRespond = new HashSet<IActorRef>(actors);
            this.Self.Tell(new StartSearch());

            Context.SetReceiveTimeout(Timeout);
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case StartSearch _:
                    this.OnStartSearch();
                    break;

                case FinishSearch _:
                    this.OnFinishSearch();
                    break;

                case GetMessages.Response response:
                    this.OnFeedResponseReceived(response.Messages);
                    break;

                case ReceiveTimeout _:
                    this.logger.Warning("Shutting down message aggregator due to timeout. Actors still to reply: ", string.Join(", ", this.actorsToRespond));
                    this.OnFinishSearch();
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private void OnFeedResponseReceived(IEnumerable<Message> responseMessages)
        {
            this.actorsToRespond.Remove(this.Sender);
            this.messages.AddRange(responseMessages);

            if (this.actorsToRespond.Count == 0)
            {
                this.Self.Tell(new FinishSearch());
            }
        }

        private void OnFinishSearch()
        {
            this.requester.Tell(new GetMessages.Response(this.messages));
            Context.Stop(this.Self);
        }

        private void OnStartSearch()
        {
            if (this.actorsToRespond.Count == 0)
            {
                this.Self.Tell(new FinishSearch());
            }

            foreach (var actorRef in this.actorsToRespond)
            {
                actorRef.Tell(this.request);
            }
        }

        private class StartSearch
        {
        }

        private class FinishSearch
        {
        }
    }
}