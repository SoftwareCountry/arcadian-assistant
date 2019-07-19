namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using CSP.Model;

    using Employees.Contracts;

    using Microsoft.EntityFrameworkCore;

    public class SickLeaveCancellationStep
    {
        private readonly ArcadiaCspContext database;

        public SickLeaveCancellationStep(ArcadiaCspContext database)
        {
            this.database = database;
        }

        public async Task InvokeAsync(EmployeeId employeeId, int eventId, EmployeeId cancelledBy)
        {
            var existingEvent = await this.database.SickLeaves.FirstOrDefaultAsync(x => x.Id == eventId && employeeId.Value == x.EmployeeId);
            if (existingEvent == null)
            {
                throw new ArgumentException($"Couldn't find {eventId} for {employeeId}");
            }

            if (existingEvent.SickLeaveCompletes.Any())
            {
                throw new ArgumentException($"Couldn't cancel {eventId} as its already complete"); //TODO: true?
            }

            this.database.SickLeaveCancellations.Add(new SickLeaveCancellations()
            {
                At = DateTimeOffset.Now,
                ById = cancelledBy.Value,
                SickLeaveId = eventId
            });

            await this.database.SaveChangesAsync();
        }
    }
}