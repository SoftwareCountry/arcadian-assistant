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
    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Remoting.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Employees : StatelessService, IEmployees
    {
        private readonly CspConfiguration cspConfiguration;
        private readonly Func<Owned<CspEmployeeQuery>> cspEmployeeQuery;
        private readonly ILogger logger;

        private readonly Expression<Func<Employee, EmployeeMetadata>> mapToMetadata;

        public Employees(
            StatelessServiceContext context, Func<Owned<CspEmployeeQuery>> cspEmployeeQuery,
            CspConfiguration cspConfiguration, ILogger<Employees> logger)
            : base(context)
        {
            this.cspEmployeeQuery = cspEmployeeQuery;
            this.cspConfiguration = cspConfiguration;
            this.logger = logger;

            this.mapToMetadata = x =>
                new EmployeeMetadata(
                    new EmployeeId(x.Id),
                    x.Email)
                {
                    LastName = x.LastName,
                    FirstName = x.FirstName,
                    BirthDate = x.Birthday,
                    HireDate = x.HiringDate,
                    FireDate = x.FiringDate,
                    MobilePhone = x.MobilePhone,
                    RoomNumber = x.RoomNumber != null ? x.RoomNumber.Trim() : null,
                    DepartmentId =
                        x.DepartmentId != null ? new DepartmentId(x.DepartmentId.Value) : (DepartmentId?)null,
                    Position = x.Position.Title,
                    Sex = x.Gender.ToUpperInvariant() == "F"
                        ? Sex.Female
                        : Sex.Male // TODO: should we use 'Undefined' for our company?
                };
        }

        public async Task<EmployeeMetadata?> FindEmployeeAsync(
            EmployeeId employeeId, CancellationToken cancellationToken)
        {
            using var query = this.cspEmployeeQuery();
            var employee = await query.Value.Get().Where(x => x.Id == employeeId.Value).Select(this.mapToMetadata)
                .FirstOrDefaultAsync(cancellationToken);

            return employee;
        }

        public async Task<EmployeeMetadata[]> FindEmployeesAsync(
            EmployeesQuery employeesQuery, CancellationToken cancellationToken)
        {
            using var db = this.cspEmployeeQuery();
            var query = db.Value.Get(employeesQuery.IncludeFired);

            if (employeesQuery.RoomNumber != null)
            {
                query = query.Where(x => x.RoomNumber.Trim() == employeesQuery.RoomNumber.Trim());
            }

            if (employeesQuery.DepartmentIds != null)
            {
                query = employeesQuery.DepartmentIds.Count == 1
                    ? query.Where(x =>
                        x.DepartmentId != null && x.DepartmentId.ToString() == employeesQuery.DepartmentIds[0])
                    : query.Where(x =>
                        x.DepartmentId != null && employeesQuery.DepartmentIds.Contains(x.DepartmentId.ToString()));
            }

            if (employeesQuery.NameFilter != null)
            {
                var pattern = $"%{employeesQuery.NameFilter}%";
                query = query.Where(x => EF.Functions.Like(x.LastName, pattern)
                    || EF.Functions.Like(x.FirstName, pattern));
            }

            if (employeesQuery.IsWorkedOption != null)
            {
                query = query.Where(x => x.IsWorking == employeesQuery.IsWorkedOption.Value);
            }

            if (employeesQuery.HireDateRange != null)
            {
                var startDate = employeesQuery.HireDateRange.Value.Item1;
                var endDate = employeesQuery.HireDateRange.Value.Item2;
                query = query.Where(x => x.HiringDate >= new DateTime(x.HiringDate.Year, startDate.Month, startDate.Day)
                    && x.HiringDate <= new DateTime(x.HiringDate.Year, endDate.Month, endDate.Day));
            }

            if (employeesQuery.BirthdayRange != null)
            {
                var startDate = employeesQuery.BirthdayRange.Value.Item1;
                var endDate = employeesQuery.BirthdayRange.Value.Item2;
                query = query.Where(x => x.Birthday.HasValue &&
                    x.Birthday >= new DateTime(x.Birthday.Value.Year, startDate.Month, startDate.Day)
                    && x.Birthday <= new DateTime(x.Birthday.Value.Year, endDate.Month, endDate.Day));
            }

            if (employeesQuery.Identity != null)
            {
                var email = employeesQuery.Identity;
                var loginName = this.ExtractLoginName(email);
                if (loginName == null)
                {
                    query = query.Where(x => EF.Functions.Like(x.Email, email));
                }
                else
                {
                    query = query.Where(x =>
                        EF.Functions.Like(x.Email, email) || EF.Functions.Like(x.LoginName, loginName));
                }
            }

            var employees = await query.Select(this.mapToMetadata).ToArrayAsync(cancellationToken);

            return employees;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        private string? ExtractLoginName(string email)
        {
            var domain = "@" + this.cspConfiguration.UserIdentityDomain;

            if (email.EndsWith(domain, StringComparison.OrdinalIgnoreCase))
            {
                return email.Substring(0, email.LastIndexOf(domain, StringComparison.OrdinalIgnoreCase));
            }

            return null;
        }
    }
}