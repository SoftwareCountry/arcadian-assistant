namespace Arcadia.Assistant.Feeds.Messages
{
    public sealed class PostMessage
    {
        public Message Message { get; }

        public PostMessage(Message message)
        {
            this.Message = message;
        }
    }
}