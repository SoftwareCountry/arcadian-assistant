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

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class Organization : StatefulService, IOrganization
    {
        private readonly Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery;
        private readonly IEmployees employees;
        private readonly ILogger logger = new LoggerFactory().CreateLogger<Organization>();

        public Organization(StatefulServiceContext context, Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery, IEmployees employees)
            : base(context)
        {
            this.allDepartmentsQuery = allDepartmentsQuery;
            this.employees = employees;
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

        public async Task<DepartmentMetadata> GetDepartmentAsync(string departmentId, CancellationToken cancellationToken)
        {
            var allDepartments = await this.GetDepartmentsAsync(cancellationToken);
            return allDepartments.FirstOrDefault(x => StringComparer.InvariantCultureIgnoreCase.Equals(x.DepartmentId, departmentId));
        }

        public async Task<DepartmentMetadata[]> GetDepartmentsAsync(CancellationToken cancellationToken)
        {
            using (var departmentsQuery = this.allDepartmentsQuery())
            {
                var departments = await departmentsQuery.Value.LoadAllAsync(cancellationToken);
                return departments.ToArray();
            }
        }

        public async Task<EmployeeMetadata> FindEmployeeSupervisorAsync(string employeeId, CancellationToken cancellationToken)
        {
            var target = await this.employees.FindEmployeeAsync(employeeId, cancellationToken);
            if (target == null)
            {
                return null;
            }

            var departments = await this.GetDepartmentsAsync(cancellationToken);

            var candidateDepartment = target.DepartmentId;
            var candidate = target;

            while (true)
            {
                var department = departments.FirstOrDefault(x => x.DepartmentId == candidateDepartment);
                if (department == null)
                {
                    // if supposed supervisor department is not found, we have no choice but to finish search
                    return candidate;
                }

                var chief = await this.employees.FindEmployeeAsync(department.ChiefId, cancellationToken);

                if (chief != null && chief.EmployeeId != candidate.EmployeeId || department.IsHeadDepartment)
                {
                    // we found the boss
                    return chief;
                }

                if (chief != null)
                {
                    candidate = chief;
                }

                //we go one level up
                candidateDepartment = department.ParentDepartmentId;
            }
        }
    }
}