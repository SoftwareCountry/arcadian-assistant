namespace Arcadia.Assistant.Web.Users
{
    using System.Linq;
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Organization.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions.OrganizationRequests;
    using Arcadia.Assistant.Web.Employees;

    public class UserEmployeeSearch : IUserEmployeeSearch
    {
        private readonly IEmployeesRegistry employeesRegistry;

        public UserEmployeeSearch(IEmployeesRegistry employeesRegistry)
        {
            this.employeesRegistry = employeesRegistry;
        }

        public async Task<EmployeeContainer> FindOrDefaultAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            if (!user.Identity.IsAuthenticated)
            {
                return null;
            }

            var query = EmployeesQuery.Create().WithIdentity(user.Identity.Name);
            var employees = await this.employeesRegistry.SearchAsync(query, cancellationToken);

            return employees.SingleOrDefault();
        }
    }
}