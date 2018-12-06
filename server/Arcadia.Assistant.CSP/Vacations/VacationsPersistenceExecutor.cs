namespace Arcadia.Assistant.CSP.Vacations
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using Arcadia.Assistant.CSP.Model;

    public class VacationsPersistenceExecutor
    {
        private readonly Func<ArcadiaCspContext> contextFactory;

        public VacationsPersistenceExecutor(Func<ArcadiaCspContext> contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<Vacation> UpsertVacation(Vacation vacation)
        {
            using (var context = this.contextFactory())
            {
                var existingVacation = await this.GetVacation(
                    context,
                    vacation.EmployeeId,
                    vacation.Start,
                    vacation.End);

                if (existingVacation == null)
                {
                    existingVacation = (await context.Vacations.AddAsync(vacation)).Entity;
                }
                else
                {
                    existingVacation.CancelledById = vacation.CancelledById;
                    existingVacation.CancelledAt = vacation.CancelledAt;

                    foreach (var approval in vacation.VacationApprovals)
                    {
                        var existingApproval = existingVacation.VacationApprovals
                            .FirstOrDefault(va => va.ApproverId == approval.ApproverId);
                        if (existingApproval == null)
                        {
                            existingVacation.VacationApprovals.Add(approval);
                        }
                        else
                        {
                            existingApproval.Status = approval.Status;

                            if (existingApproval.TimeStamp == null)
                            {
                                existingApproval.TimeStamp = approval.TimeStamp;
                            }
                        }
                    }

                    context.Vacations.Update(existingVacation);
                }

                await context.SaveChangesAsync();
                return existingVacation;
            }
        }

        private Task<Vacation> GetVacation(
            ArcadiaCspContext context,
            int employeeId,
            DateTime startDate,
            DateTime endDate)
        {
            return context.Vacations
                .Include(v => v.VacationApprovals)
                .FirstOrDefaultAsync(v =>
                    v.EmployeeId == employeeId &&
                    v.Start.Date == startDate.Date &&
                    v.End.Date == endDate.Date);
        }
    }
}