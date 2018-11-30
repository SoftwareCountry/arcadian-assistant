namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    using OrganizationRequests;

    public abstract class VacationApprovalsChecker : UntypedActor, ILogReceive
    {
        public static Props GetProps()
        {
            return Context.DI().Props<VacationApprovalsChecker>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetNextVacationRequestApprover msg:
                    this.GetNextApprover(msg.EmployeeId, msg.ExistingApprovals)
                        .PipeTo(
                            this.Sender,
                            success: r => new GetNextVacationRequestApprover.SuccessResponse(r),
                            failure: err => new GetNextVacationRequestApprover.ErrorResponse(err.Message));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals);
    }
}