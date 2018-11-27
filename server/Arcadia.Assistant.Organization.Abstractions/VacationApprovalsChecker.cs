namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Akka.Actor;

    using OrganizationRequests;

    public abstract class VacationApprovalsChecker : UntypedActor, ILogReceive
    {
        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetNextVacationRequestApprover msg:
                    this.GetNextApprover(msg.EmployeeId, msg.ExistingApprovals)
                        .PipeTo(this.Sender, success: r => new GetNextVacationRequestApprover.Response(r));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals);
    }
}