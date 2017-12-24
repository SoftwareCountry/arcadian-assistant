namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;

    using Arcadia.Assistant.Images;
    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeActor : UntypedActor
    {
        private readonly EmployeeStoredInformation employeeStoredInformation;

        public EmployeeActor(EmployeeStoredInformation storedInformation)
        {
            this.employeeStoredInformation = storedInformation;

            var photo = Context.ActorOf(Akka.Actor.Props.Create(() => new PhotoActor()), "photo");
            photo.Tell(new PhotoActor.SetSource(storedInformation.Photo));
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetEmployeeInfo _:
                    this.Sender.Tell(new GetEmployeeInfo.Response(new EmployeeContainer(this.employeeStoredInformation, this.Self)));
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