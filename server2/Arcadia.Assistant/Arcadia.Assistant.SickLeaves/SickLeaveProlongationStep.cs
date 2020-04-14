namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;

    using Permissions.Contracts;

    public class SickLeaveProlongationStep
    {
        //private readonly ArcadiaCspContext database;
        private readonly PermissionsEntryQuery permissionsQuery;
        private readonly ILogger<SickLeaveProlongationStep> logger;

        public SickLeaveProlongationStep(/*ArcadiaCspContext database,*/ PermissionsEntryQuery permissionsQuery, ILogger<SickLeaveProlongationStep> logger)
        {
            //this.database = database;
            this.permissionsQuery = permissionsQuery;
            this.logger = logger;
        }

        public async Task InvokeAsync(EmployeeId employeeId, int eventId, DateTime endDate, UserIdentity userIdentity)
        {
            var (_, entry) = await this.permissionsQuery.ExecuteAsync(userIdentity, employeeId, CancellationToken.None);
            if (!entry.HasFlag(EmployeePermissionsEntry.EditPendingCalendarEvents))
            {
                this.logger.LogError("{User} has no permissions to prolong calendar events for id {EmployeeId}", userIdentity, employeeId);
                throw new NotEnoughPermissionsException($"{userIdentity} has no permissions to create calendar events for {employeeId}");
            }
            /*
            var existingEvent = this.database.SickLeaves
                //.Include(x => x.SickLeaveCancellations)
                //.Include(x => x.SickLeaveCompletes)
                .FirstOrDefault(x => x.Id == eventId && x.EmployeeId == employeeId.Value);

            if (existingEvent == null)
            {
                throw new ArgumentException($"Existing event {eventId} not found");
            }
            
            if (new SickLeaveModelConverter().GetStatus(existingEvent) != SickLeaveStatus.Requested)
            {
                throw new ArgumentException($"Couldn't cancel {eventId} as its already complete / cancelled");
            }

            if (endDate < existingEvent.End)
            {
                throw new ArgumentException($"New end date is earlier than existing one");
            }

            existingEvent.End = endDate;
            */
            //await this.database.SaveChangesAsync();
        }
    }
}