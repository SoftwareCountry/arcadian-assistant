namespace Arcadia.Assistant.Feeds
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
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

                    //TODO: Broadcast new message information to hubs/etc
                    var newEvent = new MessageIsPostedToFeedEvent()
                        {
                            MessageId = postMessage.Message.MessageId,
                            EmployeeId = postMessage.Message.EmployeeId,
                            PostedDate = postMessage.Message.DatePosted,
                            Text = postMessage.Message.Text,
                            Title = postMessage.Message.Title
                        };

                    this.Persist(newEvent, this.MessagePosted);
                    break;

                case GetMessages _:
                    this.Sender.Tell(new GetMessages.Response(this.messagesList));

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
            this.messagesList.Add(new Message(e.MessageId, e.EmployeeId, e.Title, e.Text, e.PostedDate));
        }

        public sealed class PostMessage
        {
            public Message Message { get; }

            public PostMessage(Message message)
            {
                this.Message = message;
            }
        }

        public sealed class GetMessages
        {
            public sealed class Response
            {
                public Response(IEnumerable<Message> messages)
                {
                    this.Messages = messages.ToList();
                }

                public IReadOnlyCollection<Message> Messages { get; }
            }
        }

        public static Props CreateProps(string feedId) => Props.Create(() => new FeedActor(feedId));
    }
}