namespace Arcadia.Assistant.Feeds
{
    using System;

    using Akka.Actor;
    using Akka.Pattern;
    using Akka.Persistence;

    using Arcadia.Assistant.Feeds.Messages;
    using Arcadia.Assistant.Patterns;

    public class PersistentFeedActor : UntypedPersistentActor
    {
        private readonly IActorRef internalFeed;

        public PersistentFeedActor(string feedName)
        {
            this.PersistenceId = feedName;
            this.internalFeed = Context.ActorOf(Props.Create(() => new FeedActor()));
        }

        public override string PersistenceId { get; }

        protected override void OnCommand(object cmd)
        {
            switch (cmd)
            {
                case PostMessage postMessage:

                    //TODO: Broadcast new message information to hubs/etc
                    var newEvent = new MessageIsPostedToFeedEvent()
                    {
                        MessageId = postMessage.Message.MessageId,
                        EmployeeId = postMessage.Message.PostedByEmployeeId,
                        PostedDate = postMessage.Message.DatePosted,
                        Text = postMessage.Message.Text,
                        Title = postMessage.Message.Title
                    };

                    this.Persist(newEvent, this.MessagePosted);
                    break;

                case GetMessages msg:
                    this.internalFeed.Forward(msg);
                    break;

                default:
                    this.Unhandled(cmd);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case MessageIsPostedToFeedEvent e:
                    this.MessagePosted(e);
                    break;
            }
        }

        private void MessagePosted(MessageIsPostedToFeedEvent e)
        {
            this.internalFeed.Tell(new PostMessage(new Message(e.MessageId, e.EmployeeId, e.Title, e.Text, e.PostedDate)));
        }

        public static Props CreateProps(string feedId) => Props.Create(() => new PersistentFeedActor(feedId));
    }
}