namespace Arcadia.Assistant.Organization
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using Employees.Contracts;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Data.Collections;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class Organization : StatefulService, IOrganization
    {
        private readonly ISupervisorSearch supervisorSearch;
        private readonly IOrganizationDepartments organizationDepartments;
        private readonly ILogger logger = new LoggerFactory().CreateLogger<Organization>();

        public Organization(StatefulServiceContext context, Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery, IEmployees employees)
            : base(context)
        {
            this.organizationDepartments = new CachedOrganizationDepartments(this.StateManager, allDepartmentsQuery);
            this.supervisorSearch = new SupervisorSearch(employees, this.organizationDepartments);
        }

        public async Task<DepartmentMetadata> GetDepartmentAsync(string departmentId, CancellationToken cancellationToken)
        {
            var allDepartments = await this.GetDepartmentsAsync(cancellationToken);
            return allDepartments.FirstOrDefault(x => StringComparer.InvariantCultureIgnoreCase.Equals(x.DepartmentId, departmentId));
        }

        public Task<DepartmentMetadata[]> GetDepartmentsAsync(CancellationToken cancellationToken)
        {
            return this.organizationDepartments.GetAllAsync(cancellationToken);
        }

        public Task<EmployeeMetadata> FindEmployeeSupervisorAsync(string employeeId, CancellationToken cancellationToken)
        {
            return this.supervisorSearch.FindAsync(employeeId, cancellationToken);
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle
        ///     client or user requests.
        /// </summary>
        /// <remarks>
        ///     For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }
    }
}