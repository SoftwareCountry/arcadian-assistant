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
        private readonly IEmployeesSearch employeesSearch;

        public MockUserEmployeeSearch(IEmployeesSearch employeesSearch)
        {
            this.employeesSearch = employeesSearch;
        }

        public async Task<EmployeeContainer> FindOrDefault(ClaimsPrincipal user, CancellationToken cancellationToken)
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
            var employees = await this.employeesSearch.Search(query, cancellationToken);

            if (!employees.Any())
            {
                //TODO: temp measure, before all users are in database
                query = EmployeesQuery.Create().WithEmail(defaultEmail);
                employees = await this.employeesSearch.Search(query, cancellationToken);
            }

            return employees.SingleOrDefault();
        }
    }
}