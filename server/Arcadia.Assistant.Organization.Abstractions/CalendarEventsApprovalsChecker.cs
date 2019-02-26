namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.DI.Core;

    using OrganizationRequests;

    public abstract class CalendarEventsApprovalsChecker : UntypedActor, ILogReceive
    {
        public static Props GetProps()
        {
            return Context.DI().Props<CalendarEventsApprovalsChecker>();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetNextCalendarEventApprover msg:
                    this.GetNextApprover(msg.EmployeeId, msg.ExistingApprovals, msg.EventType)
                        .PipeTo(
                            this.Sender,
                            success: r => new GetNextCalendarEventApprover.SuccessResponse(r),
                            failure: err => new GetNextCalendarEventApprover.ErrorResponse(err.ToString()));
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected abstract Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals, string eventType);
    }
}