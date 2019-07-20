namespace Arcadia.Assistant.Web.DepartmentFeatures
{
    using System.Threading;
    using System.Threading.Tasks;

    using Arcadia.Assistant.Web.Models;

    public interface IDepartmentFeaturesService
    {
        Task<DepartmentFeaturesModel> GetDepartmentFeatures(string departmentId, CancellationToken cancellationToken);
    }
}