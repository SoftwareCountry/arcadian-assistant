namespace Arcadia.Assistant.Organization
{
    using Autofac.Features.OwnedInstances;
    using Contracts;

    using Employees.Contracts;
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CSP.WebApi.Contracts;
    using CSP.WebApi.Contracts.Models;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class Organization : StatefulService, IOrganization
    {
        //private readonly ISupervisorSearch supervisorSearch;
        //private readonly IOrganizationDepartments organizationDepartments;
        private readonly ICspApi csp;
        private readonly ILogger logger;

        public Organization(StatefulServiceContext context, ICspApi csp,/*  Func<Owned<OrganizationDepartmentsQuery>> allDepartmentsQuery,*/ IEmployees employees, ILogger<Organization> logger)
            : base(context)
        {
            //this.organizationDepartments = new CachedOrganizationDepartments(this.StateManager, allDepartmentsQuery, this.logger);
            //this.supervisorSearch = new SupervisorSearch(employees, this.organizationDepartments);
            this.csp = csp;
            this.logger = logger;
        }

        public async Task<DepartmentMetadata?> GetDepartmentAsync(DepartmentId departmentId, CancellationToken cancellationToken)
        {
            var allDepartments = await this.GetDepartmentsAsync(cancellationToken);
            return allDepartments.FirstOrDefault(x => x.DepartmentId == departmentId);
        }

        public async Task<DepartmentMetadata[]> GetDepartmentsAsync(CancellationToken cancellationToken)
        {
            return (await this.csp.GetDepartmentWithPeople(cancellationToken))
                .Select(this.MapToDepartmentMetadata)
                .ToArray();
            //return this.organizationDepartments.GetAllAsync(cancellationToken);
        }

        public Task<EmployeeMetadata?> FindEmployeeSupervisorAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            //return this.supervisorSearch.FindAsync(employeeId, cancellationToken);
            return Task.FromResult<EmployeeMetadata?>(null);
        }

        public async Task<DepartmentMetadata[]> GetSupervisedDepartmentsAsync(EmployeeId employeeId, CancellationToken cancellationToken)
        {
            var allDepartments = await this.GetDepartmentsAsync(cancellationToken);
            var supervisedDepartmentsSearch = new SupervisedDepartmentsSearch(allDepartments);
            return supervisedDepartmentsSearch.FindFor(employeeId);
        }

        private DepartmentMetadata MapToDepartmentMetadata(DepartmentWithPeopleCount x)
        {
            return new DepartmentMetadata(
                new DepartmentId(x.Department.Id),
                x.Department.Name,
                x.Department.Abbreviation,
                x.Department.ParentDepartmentId == null || x.Department.ParentDepartmentId == x.Department.Id
                    ? (DepartmentId?)null
                    : new DepartmentId(x.Department.ParentDepartmentId.Value))
            {
                ChiefId = x.ActualChiefId == null ? (EmployeeId?)(null) : new EmployeeId(x.ActualChiefId.Value),
                PeopleCount = x.PeopleCount
            };
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