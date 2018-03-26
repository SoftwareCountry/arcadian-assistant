namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;
    using Akka.Persistence;

    using Arcadia.Assistant.Feeds.Messages;

    public class PersistentFeedActor : UntypedPersistentActor
    {
        private readonly List<Message> messagesList = new List<Message>();

        public PersistentFeedActor(string feedName)
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

                case GetMessages msg:
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

        public static Props CreateProps(string feedId) => Props.Create(() => new PersistentFeedActor(feedId));
    }
}