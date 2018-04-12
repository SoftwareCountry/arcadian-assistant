namespace Arcadia.Assistant.Feeds
{
    using System.Collections.Generic;
    using System.Linq;

    using Akka.Actor;

    using Arcadia.Assistant.Feeds.Messages;

    public class FeedActor : UntypedActor
    {
        private readonly List<Message> messagesList = new List<Message>();

        protected override void OnReceive(object cmd)
        {
            switch (cmd)
            {
                case PostMessage postMessage:
                    //TODO: Broadcast new message information to hubs/etc
                    var message = postMessage.Message;
                    this.messagesList.Add(new Message(message.MessageId, message.EmployeeId, message.Title, message.Text, message.DatePosted));
                    break;

                case GetMessages request:
                    var messages = this.messagesList.Where(x => (x.DatePosted.Date >= request.FromDate.Date) && (x.DatePosted.Date <= request.ToDate.Date));
                    this.Sender.Tell(new GetMessages.Response(messages));
                    break;
            }
        }
    }
}