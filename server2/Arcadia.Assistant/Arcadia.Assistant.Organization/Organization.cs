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

    using CSP;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public class Organization : StatefulService, IOrganization
    {
        private readonly Func<Owned<CspEmployeeQuery>> cspEmployeeQuery;

        public Organization(StatefulServiceContext context, Func<Owned<CspEmployeeQuery>> cspEmployeeQuery)
            : base(context)
        {
            this.cspEmployeeQuery = cspEmployeeQuery;
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

        public async Task<EmployeeMetadata> FindByIdAsync(string employeeId, CancellationToken cancellationToken)
        {
            using (var query = this.cspEmployeeQuery())
            {
                if (!int.TryParse(employeeId, out var id))
                {
                    return null;
                }

                var employee = await query.Value.Get().FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
                if (employee == null)
                {
                    return null;
                }

                return new EmployeeMetadata(employeeId, employee.LastName, employee.Email);
            }
        }

        public async Task<EmployeeMetadata[]> FindEmployeesAsync(EmployeesQuery employeesQuery, CancellationToken cancellationToken)
        {
            using (var db = this.cspEmployeeQuery())
            {
                var query = db.Value.Get();

                if (employeesQuery.RoomNumber != null)
                {
                    query = query.Where(x => x.RoomNumber.Trim() == employeesQuery.RoomNumber.Trim());
                }

                if (employeesQuery.DepartmentId != null)
                {
                    query = query.Where(x => x.DepartmentId.ToString() == employeesQuery.DepartmentId);
                }

                if (employeesQuery.NameFilter != null)
                {
                    query = query.Where(x => x.LastName.Contains(employeesQuery.NameFilter, StringComparison.InvariantCultureIgnoreCase) 
                        || x.FirstName.Contains(employeesQuery.NameFilter, StringComparison.InvariantCultureIgnoreCase));
                }

                var userIdentityDomain = "arcadia.sbb.ru"; //TODO: hardcode

                var employees = await query.Select(x => 
                    new EmployeeMetadata(
                        x.Id.ToString(),
                        $"{x.LastName} {x.FirstName}".Trim(),
                        x.Email,
                        new [] { x.Email, $"{x.LoginName}@{userIdentityDomain}" })
                    {
                        BirthDate = x.Birthday,
                        HireDate = x.HiringDate,
                        FireDate = x.FiringDate,
                        MobilePhone = x.MobilePhone,
                        RoomNumber = x.RoomNumber != null ? x.RoomNumber.Trim() : null,

                    }) 
                    .ToArrayAsync(cancellationToken);

                return employees;
            }
        }
    }
}