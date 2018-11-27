namespace Arcadia.Assistant.CSP.Vacations
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Organization.Abstractions;

    public class CspVacationApprovalsChecker : VacationApprovalsChecker
    {
        protected override async Task<string> GetNextApprover(string employeeId, IEnumerable<string> existingApprovals)
        {
            return string.Empty;
        }
    }
}