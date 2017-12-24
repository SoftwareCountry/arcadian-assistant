namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public class EmployeeActor : UntypedActor
    {
        private readonly EmployeeStoredInformation employeeStoredInformation;

        public EmployeeActor(EmployeeStoredInformation storedInformation)
        {
            this.employeeStoredInformation = storedInformation;
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