namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;

    using Arcadia.Assistant.Feeds;
    using Arcadia.Assistant.Images;
    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeActor : UntypedActor
    {
        private readonly EmployeeMetadata employeeMetadata;

        private readonly IActorRef photo;

        private readonly IActorRef employeeFeed;

        public EmployeeActor(EmployeeStoredInformation storedInformation)
        {
            this.employeeMetadata = storedInformation.Metadata;

            this.photo = Context.ActorOf(Akka.Actor.Props.Create(() => new PhotoActor()), "photo");
            this.photo.Tell(new PhotoActor.SetSource(storedInformation.Photo));

            this.employeeFeed = Context.ActorOf(FeedActor.CreateProps(this.employeeMetadata.EmployeeId), "feed");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetEmployeeInfo _:
                    this.Sender.Tell(new GetEmployeeInfo.Response(new EmployeeContainer(this.employeeMetadata, this.Self)));
                    break;

                case GetPhoto _:
                    this.photo.Forward(message);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public class GetEmployeeInfo
        {
            public static readonly GetEmployeeInfo Instance = new GetEmployeeInfo();

            private GetEmployeeInfo() { }

            public class Response
            {
                public Response(EmployeeContainer employee)
                {
                    this.Employee = employee;
                }

                public EmployeeContainer Employee { get; }
            }
        }

        public static Props Props(EmployeeStoredInformation employeeStoredInformation) => Akka.Actor.Props.Create(() => new EmployeeActor(employeeStoredInformation));
    }
}