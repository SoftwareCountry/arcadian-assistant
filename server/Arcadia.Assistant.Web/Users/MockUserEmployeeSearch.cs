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

    public class MockUserEmployeeSearch : IUserEmployeeSearch
    {
        private readonly IEmployeesRegistry employeesRegistry;

        public MockUserEmployeeSearch(IEmployeesRegistry employeesRegistry)
        {
            this.employeesRegistry = employeesRegistry;
        }

        public async Task<EmployeeContainer> FindOrDefaultAsync(ClaimsPrincipal user, CancellationToken cancellationToken)
        {
            string email;
            var defaultEmail = "alexander.shevnin@arcadia.spb.ru";

            //TODO: temp assignment, before authentication works
            if (!user.Identity.IsAuthenticated)
            {
                email = defaultEmail;
            }
            else
            {
                email = user.Identity.Name;
            }

            var query = EmployeesQuery.Create().WithEmail(email);
            var employees = await this.employeesRegistry.SearchAsync(query, cancellationToken);

            if (!employees.Any())
            {
                //TODO: temp measure, before all users are in database
                query = EmployeesQuery.Create().WithEmail(defaultEmail);
                employees = await this.employeesRegistry.SearchAsync(query, cancellationToken);
            }

            return employees.SingleOrDefault();
        }
    }
}