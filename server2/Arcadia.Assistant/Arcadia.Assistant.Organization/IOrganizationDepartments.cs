namespace Arcadia.Assistant.Organization
{
    using Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IOrganizationDepartments
    {
        Task<DepartmentMetadata[]> GetAllAsync(CancellationToken cancellationToken);
    }
}