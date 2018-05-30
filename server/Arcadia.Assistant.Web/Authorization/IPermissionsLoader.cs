using System.Security.Claims;
using System.Threading.Tasks;
using Arcadia.Assistant.Security;

namespace Arcadia.Assistant.Web.Authorization
{
    public interface IPermissionsLoader
    {
        Task<Permissions> LoadAsync(ClaimsPrincipal user);
    }
}