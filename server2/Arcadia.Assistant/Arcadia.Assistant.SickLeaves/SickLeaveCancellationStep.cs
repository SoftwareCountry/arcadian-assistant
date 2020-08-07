namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using CSP.Model;

    using Employees.Contracts;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;

    using Permissions.Contracts;

    public class SickLeaveCancellationStep
    {
        private readonly ArcadiaCspContext database;
        private readonly ILogger<SickLeaveCreationStep> logger;
        private readonly PermissionsEntryQuery permissionsQuery;

        public SickLeaveCancellationStep(
            ArcadiaCspContext database, PermissionsEntryQuery permissionsQuery, ILogger<SickLeaveCreationStep> logger)
        {
            this.database = database;
            this.permissionsQuery = permissionsQuery;
            this.logger = logger;
        }

        /// <exception cref="NotEnoughPermissionsException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public async Task InvokeAsync(EmployeeId employeeId, int eventId, UserIdentity cancelledBy)
        {
            var (cancelledById, permissions) =
                await this.permissionsQuery.ExecuteAsync(cancelledBy, employeeId, CancellationToken.None);

            if (!permissions.HasFlag(EmployeePermissionsEntry.CancelPendingCalendarEvents))
            {
                this.logger.LogError(
                    "User {User} has no permissions to cancel sick leave on employee {Employee}. Permissions are {Permissions}",
                    employeeId, eventId, permissions);
                throw new NotEnoughPermissionsException(
                    $"User {cancelledBy} has no permissions to create events on employee {employeeId}");
            }

            if (cancelledById == null)
            {
                throw new InvalidOperationException($"Can't setup cancelledBy, employee not found for {cancelledBy}");
            }

            var existingEvent =
                await this.database.SickLeaves
                    .Include(x => x.SickLeaveCancellations)
                    .Include(x => x.SickLeaveCompletes)
                    .FirstOrDefaultAsync(x =>
                        x.Id == eventId && employeeId.Value == x.EmployeeId);

            if (existingEvent == null)
            {
                throw new ArgumentException($"Couldn't find {eventId} for {employeeId}");
            }

            var status = new SickLeaveModelConverter().GetStatus(existingEvent);

            if (status != SickLeaveStatus.Requested)
            {
                throw new ArgumentException($"Couldn't cancel {eventId} as its already complete / cancelled");
            }

            existingEvent.SickLeaveCancellations.Add(new SickLeaveCancellation
            {
                At = DateTimeOffset.Now, ById = cancelledById.Value.Value, SickLeaveId = existingEvent.Id
            });

            await this.database.SaveChangesAsync();
        }
    }
}