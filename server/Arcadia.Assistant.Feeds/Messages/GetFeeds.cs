namespace Arcadia.Assistant.Feeds.Messages
{
    using System.Collections.Generic;

    using Akka.Actor;

    public sealed class GetFeeds
    {
        public string EmployeeId { get; }

        public GetFeeds(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public sealed class Response
        {
            public IReadOnlyDictionary<string, IActorRef> Feeds { get; }

            public Response(IReadOnlyDictionary<string, IActorRef> feeds)
            {
                this.Feeds = feeds;
            }
        }
    }
}