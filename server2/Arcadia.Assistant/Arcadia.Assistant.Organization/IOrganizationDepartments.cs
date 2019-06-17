namespace Arcadia.Assistant.Organization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Contracts;

    public interface IOrganizationDepartments
    {
        Task<DepartmentMetadata[]> GetAllAsync(CancellationToken cancellationToken);
    }
}