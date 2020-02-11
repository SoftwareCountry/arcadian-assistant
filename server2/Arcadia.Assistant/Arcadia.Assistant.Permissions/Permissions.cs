namespace Arcadia.Assistant.Permissions
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Fabric;
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Organization.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Permissions : StatelessService, IPermissions
    {
        private readonly IEmployees employees;
        private readonly IOrganization organization;
        private readonly ILogger logger;

        public Permissions(StatelessServiceContext context, IEmployees employees, IOrganization organization, ILogger logger)
            : base(context)
        {
            this.employees = employees;
            this.organization = organization;
            this.logger = logger;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        public async Task<UserPermissionsCollection> GetPermissionsAsync(string identity, CancellationToken cancellationToken)
        {
            var userEmployee = (await this.employees.FindEmployeesAsync(EmployeesQuery.Create().WithIdentity(identity), cancellationToken)).FirstOrDefault();
            var defaultEmployeePermission = EmployeePermissionsEntry.None;
            var permissionsForDepartments = new Dictionary<DepartmentId, EmployeePermissionsEntry>();
            var permissionsForEmployees = new Dictionary<EmployeeId, EmployeePermissionsEntry>();

            if (userEmployee != null)
            {
                defaultEmployeePermission = ExistingEmployeeDefaultPermission;
                BulkBumpPermissions(new [] { userEmployee.EmployeeId }, SelfPermissions, permissionsForEmployees);

                if (userEmployee.DepartmentId.HasValue)
                {
                    BulkBumpPermissions(new[] { userEmployee.DepartmentId.Value }, OwnDepartmentPermissions, permissionsForDepartments);
                }

                var supervisedDepartments = await this.organization.GetSupervisedDepartmentsAsync(userEmployee.EmployeeId, cancellationToken);
                BulkBumpPermissions(supervisedDepartments.Select(x => x.DepartmentId), SupervisedPermissions, permissionsForDepartments);
            }

            return new UserPermissionsCollection(defaultEmployeePermission, permissionsForDepartments, permissionsForEmployees);
        }

        private static void BulkBumpPermissions<T>(IEnumerable<T> entriesIds,
            EmployeePermissionsEntry permissionSet,
            Dictionary<T, EmployeePermissionsEntry> targetPermissionsEntries)
        where T: struct
        {
            foreach (var entryId in entriesIds)
            {
                if (targetPermissionsEntries.ContainsKey(entryId))
                {
                    targetPermissionsEntries[entryId] |= permissionSet;
                }
                else
                {
                    targetPermissionsEntries[entryId] = permissionSet;
                }
            }
        }

        private static EmployeePermissionsEntry ExistingEmployeeDefaultPermission =
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents;

        private static readonly EmployeePermissionsEntry OwnDepartmentPermissions =
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeePhone;

        private static readonly EmployeePermissionsEntry SelfPermissions =
            EmployeePermissionsEntry.CreateCalendarEvents |
            EmployeePermissionsEntry.CompleteSickLeave |
            EmployeePermissionsEntry.ProlongSickLeave |
            EmployeePermissionsEntry.CancelPendingCalendarEvents |
            EmployeePermissionsEntry.EditPendingCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeePhone |
            EmployeePermissionsEntry.ReadEmployeeDayoffsCounter |
            EmployeePermissionsEntry.ReadEmployeeVacationsCounter;

        private static readonly EmployeePermissionsEntry SupervisedPermissions =
            EmployeePermissionsEntry.CreateCalendarEvents |
            EmployeePermissionsEntry.ApproveCalendarEvents |
            EmployeePermissionsEntry.RejectCalendarEvents |
            EmployeePermissionsEntry.CompleteSickLeave |
            EmployeePermissionsEntry.ProlongSickLeave |
            EmployeePermissionsEntry.CancelPendingCalendarEvents |
            EmployeePermissionsEntry.EditPendingCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeCalendarEvents |
            EmployeePermissionsEntry.ReadEmployeeInfo |
            EmployeePermissionsEntry.ReadEmployeePhone |
            EmployeePermissionsEntry.ReadEmployeeDayoffsCounter |
            EmployeePermissionsEntry.ReadEmployeeVacationsCounter |
            EmployeePermissionsEntry.CancelApprovedCalendarEvents;
    }
}