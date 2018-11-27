namespace Arcadia.Assistant.Organization.Abstractions.OrganizationRequests
{
    using System.Collections.Generic;

    public class GetNextVacationRequestApprover
    {
        public GetNextVacationRequestApprover(string employeeId, IEnumerable<string> existingApprovals)
        {
            this.EmployeeId = employeeId;
            this.ExistingApprovals = existingApprovals;
        }

        public string EmployeeId { get; }

        public IEnumerable<string> ExistingApprovals { get; }

        public class Response
        {
            public Response(string nextApproverEmployeeId)
            {
                this.NextApproverEmployeeId = nextApproverEmployeeId;
            }

            public string NextApproverEmployeeId { get; }
        }
    }
}