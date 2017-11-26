namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeActor : UntypedActor
    {
        private readonly IActorRef infoLoader;

        public EmployeeActor(string employeeId)
        {
            this.EmployeeId = employeeId;

            this.infoLoader = Context.ActorOf(EmployeeInfoQuery.Props, "info-loader");
        }

        private string EmployeeId { get; }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case OrganizationRequests.RequestEmployeeInfo request when this.EmployeeId == request.EmployeeId:
                    this.infoLoader.Forward(request);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }
    }
}