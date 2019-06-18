namespace Arcadia.Assistant.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using CSP;
    using CSP.Model;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Employees : StatelessService, IEmployees
    {
        private readonly Func<Owned<CspEmployeeQuery>> cspEmployeeQuery;
        private readonly CspConfiguration cspConfiguration;

        private readonly Expression<Func<Employee, EmployeeMetadata>> mapToMetadata;
        public Employees(StatelessServiceContext context, Func<Owned<CspEmployeeQuery>> cspEmployeeQuery, CspConfiguration cspConfiguration)
            : base(context)
        {
            this.cspEmployeeQuery = cspEmployeeQuery;
            this.cspConfiguration = cspConfiguration;

            this.mapToMetadata = x =>
                new EmployeeMetadata(
                    x.Id.ToString(),
                    $"{x.LastName} {x.FirstName}".Trim(),
                    x.Email)
                {
                    BirthDate = x.Birthday,
                    HireDate = x.HiringDate,
                    FireDate = x.FiringDate,
                    MobilePhone = x.MobilePhone,
                    RoomNumber = x.RoomNumber != null ? x.RoomNumber.Trim() : null,
                    DepartmentId = x.DepartmentId != null ? x.DepartmentId.ToString() : null,
                    Position = x.Position.Title,
                };
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }


        public async Task<EmployeeMetadata> FindEmployeeAsync(string employeeId, CancellationToken cancellationToken)
        {
            using (var query = this.cspEmployeeQuery())
            {
                if (!int.TryParse(employeeId, out var id))
                {
                    return null;
                }

                var employee = await query.Value.Get().Where(x => x.Id == id).Select(this.mapToMetadata).FirstOrDefaultAsync(cancellationToken);

                return employee;
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
                    var pattern = $"%{employeesQuery.NameFilter}%";
                    query = query.Where(x => EF.Functions.Like(x.LastName, pattern) 
                        || EF.Functions.Like(x.FirstName, pattern));
                }

                if (employeesQuery.Identity != null)
                {
                    var email = employeesQuery.Identity;
                    var loginName = this.ExtractLoginName(employeesQuery);
                    if (loginName == null)
                    {
                        query = query.Where(x => EF.Functions.Like(x.Email, email));
                    }
                    else
                    {
                        query = query.Where(x => EF.Functions.Like(x.Email, email) || EF.Functions.Like(x.LoginName, loginName));
                    }                    
                }

                var employees = await query.Select(this.mapToMetadata).ToArrayAsync(cancellationToken);

                return employees;
            }
        }

        private string ExtractLoginName(EmployeesQuery employeesQuery)
        {
            var domain = "@" + this.cspConfiguration.UserIdentityDomain;
            if (employeesQuery.Identity.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                return employeesQuery.Identity.Substring(0, employeesQuery.Identity.LastIndexOf(domain, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }
    }
}