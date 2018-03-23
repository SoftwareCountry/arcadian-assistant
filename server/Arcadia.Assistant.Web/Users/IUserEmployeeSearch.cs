namespace Arcadia.Assistant.Web.Users
{
    using System.Security.Claims;
    using System.Security.Principal;
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Organization.Abstractions;

    public interface IUserEmployeeSearch
    {
        Task<EmployeeContainer> FindOrDefault(ClaimsPrincipal user, CancellationToken cancellationToken);
    }
}