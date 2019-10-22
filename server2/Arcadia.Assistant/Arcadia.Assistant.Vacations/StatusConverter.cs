namespace Arcadia.Assistant.Vacations
{
    using System.Linq;

    using Contracts;

    public class StatusConverter
    {
        public VacationStatus GetStatus(CSP.Model.Vacation vacation)
        {
            return vacation.VacationCancellations.Any() ? VacationStatus.Cancelled
                : vacation.VacationApprovals.Any(v => v.Status == 1) ? VacationStatus.Rejected
                : vacation.VacationProcesses.Any() ? VacationStatus.Processed
                : vacation.VacationReadies.Any() ? VacationStatus.AccountingReady
                : vacation.VacationApprovals.Any(v => v.IsFinal && v.Status == 2) ? VacationStatus.Approved
                : VacationStatus.Requested;
        }
    }
}