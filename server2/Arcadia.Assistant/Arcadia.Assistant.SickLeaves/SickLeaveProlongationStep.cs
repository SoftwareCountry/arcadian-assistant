namespace Arcadia.Assistant.SickLeaves
{
    using CSP.Model;
    using Employees.Contracts;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;

    public class SickLeaveProlongationStep
    {
        private readonly ArcadiaCspContext database;

        public SickLeaveProlongationStep(ArcadiaCspContext database)
        {
            this.database = database;
        }

        public async Task InvokeAsync(EmployeeId employeeId, int eventId, DateTime endDate)
        {
            var existingEvent = await this.database.SickLeaves
                .AsTracking()
                .FirstOrDefaultAsync(x => x.Id == eventId && x.EmployeeId == employeeId.Value);

            if (existingEvent == null)
            {
                throw new ArgumentException($"Existing event {eventId} not found");
            }

            if (endDate < existingEvent.End)
            {
                throw new ArgumentException($"New end date is earlier than existing one");
            }

            existingEvent.End = endDate;
            await this.database.SaveChangesAsync();
        }
    }
}