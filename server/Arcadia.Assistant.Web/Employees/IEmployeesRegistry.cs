namespace Arcadia.Assistant.Web.Employees
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;

    public interface IEmployeesRegistry
    {
        Task<IReadOnlyCollection<EmployeeContainer>> SearchAsync(EmployeesQuery query, CancellationToken token = default(CancellationToken));
    }
}