namespace Arcadia.Assistant.Feeds
{
    using System.Collections.Generic;

    using Akka.Persistence;
    public class FeedActor : UntypedPersistentActor
    {
        private readonly List<Message> messagesList = new List<Message>();

        public FeedActor(string feedName)
        {
            this.PersistenceId = feedName;
        }

        public override string PersistenceId { get; }

        protected override void OnCommand(object cmd)
        {
            switch (cmd)
            {
                case PostMessage postMessage:
                    var newEvent = new MessageIsPostedToFeedEvent()
                        {
                            MessageId = postMessage.Message.MessageId,
                            PostedDate = postMessage.Message.DatePosted,
                            Text = postMessage.Message.Text,
                            Title = postMessage.Message.Title
                        };

                    this.Persist(newEvent, this.MessagePosted);
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
            this.messagesList.Add(new Message(e.MessageId, e.Title, e.Text, e.PostedDate));
        }

        public sealed class PostMessage
        {
            public Message Message { get; }

            public PostMessage(Message message)
            {
                this.Message = message;
            }
        }
    }
}