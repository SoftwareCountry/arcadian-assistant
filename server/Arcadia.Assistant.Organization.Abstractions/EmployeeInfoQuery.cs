namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    public abstract class EmployeeInfoQuery : UntypedActor
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case OrganizationRequests.RequestEmployeeInfo request:
                    this.GetEmployeeDemographics(request.EmployeeId).PipeTo(this.Sender);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<OrganizationRequests.RequestEmployeeInfo.Success> GetEmployeeDemographics(string employeeId);

        public static Props Props => Context.DI().Props<EmployeeInfoQuery>();
    }
}