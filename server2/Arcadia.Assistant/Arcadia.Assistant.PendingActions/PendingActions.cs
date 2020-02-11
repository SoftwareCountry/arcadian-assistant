
namespace Arcadia.Assistant.PendingActions
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Organization.Contracts;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class PendingActions : StatelessService, IPendingActions
    {
        private IVacations vacations;
        private IWorkHoursCredit workHoursCredit;
        private IEmployees employees;
        private IOrganization organization;
        private readonly ILogger logger;

        public PendingActions(StatelessServiceContext context, 
            IOrganization organization, 
            IVacations vacations, 
            IWorkHoursCredit workHoursCredit, 
            IEmployees employees,
            ILogger logger)
            : base(context)
        {
            this.organization = organization;
            this.vacations = vacations;
            this.workHoursCredit = workHoursCredit;
            this.employees = employees;
            this.logger = logger;
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        public async Task<PendingRequests> GetPendingRequestsAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            var departments = await this.organization.GetSupervisedDepartmentsAsync(employeeId, cancellationToken);
            var departmentIds = departments.Select(x => x.DepartmentId.Value.ToString()).ToArray();
            var supervisedEmployees = await this.employees.FindEmployeesAsync(EmployeesQuery.Create().ForDepartments(departmentIds), cancellationToken);
            var supervisedEmployeesIds = supervisedEmployees.Select(x => x.EmployeeId).Distinct().ToArray();

            var pendingVacationsTask = this.vacations.GetCalendarEventsByEmployeeAsync(supervisedEmployeesIds, cancellationToken);
            var pendingWorkhoursTask = this.workHoursCredit.GetActiveRequestsAsync(supervisedEmployeesIds, cancellationToken);

            await Task.WhenAll(pendingWorkhoursTask, pendingVacationsTask);

            var pendingVacations = pendingVacationsTask.Result.SelectMany(x => x.Value).ToArray();
            var pendingWorkhours = pendingWorkhoursTask.Result.SelectMany(x => x.Value).ToArray();

            return new PendingRequests() { PendingVacations = pendingVacations.ToArray(), PendingWorkHoursChanges = pendingWorkhours };
        }
    }
}
