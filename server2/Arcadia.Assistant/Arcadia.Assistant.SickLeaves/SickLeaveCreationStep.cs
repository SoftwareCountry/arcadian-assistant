namespace Arcadia.Assistant.SickLeaves
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using CSP.Contracts;
    using CSP.Contracts.Models;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;

    using Permissions.Contracts;

    public class SickLeaveCreationStep
    {
        private readonly ArcadiaCspContext context;
        private readonly PermissionsEntryQuery permissionsQuery;
        private readonly ILogger<SickLeaveCreationStep> logger;
        private readonly SickLeaveModelConverter converter = new SickLeaveModelConverter();

        public SickLeaveCreationStep(ArcadiaCspContext context, PermissionsEntryQuery permissionsQuery, ILogger<SickLeaveCreationStep> logger)
        {
            this.context = context;
            this.permissionsQuery = permissionsQuery;
            this.logger = logger;
        }

        /// <exception cref="NotEnoughPermissionsException"></exception>
        public async Task<SickLeaveDescription> InvokeAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate, UserIdentity creatorIdentity)
        {
            using var scope = this.logger.BeginScope("New sick leave creation for {EmployeeId}, {StartDate} - {EndDate}", employeeId, startDate, endDate);
            var (_, entry) = await this.permissionsQuery.ExecuteAsync(creatorIdentity, employeeId, CancellationToken.None);
            if (!entry.HasFlag(EmployeePermissionsEntry.CreateCalendarEvents))
            {
                this.logger.LogError("{User} has no permissions to create calendar events for id {EmployeeId}", creatorIdentity, employeeId);
                throw new NotEnoughPermissionsException($"{creatorIdentity} has no permissions to create calendar events for {employeeId}");
            }

            var newSickLeave = new SickLeave()
            {
                EmployeeId = employeeId.Value,
                Start = startDate,
                End = endDate,
                RaisedAt = DateTimeOffset.Now
            };

            this.context.SickLeaves.Add(newSickLeave);
            //await this.context.SaveChangesAsync();

            this.logger.LogInformation("Sick leave {SickLeave} created", newSickLeave);

            return this.converter.GetDescription(newSickLeave);
        }
    }
}