namespace Arcadia.Assistant.Organization
{
    using Akka.Actor;
    using Akka.DI.Core;

    using Arcadia.Assistant.Organization.Abstractions;

    public class EmployeeActor : UntypedActor
    {
        private readonly EmployeeInfo employeeInfo;

        public EmployeeActor(EmployeeInfo info)
        {
            this.employeeInfo = info;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case OrganizationRequests.RequestEmployeeInfo request when this.employeeInfo.EmployeeId == request.EmployeeId:
                    this.Sender.Tell(new OrganizationRequests.RequestEmployeeInfo.Success(this.employeeInfo));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        public static Props Props(EmployeeInfo employeeInfo) => Akka.Actor.Props.Create(() => new EmployeeActor(employeeInfo));
    }
}