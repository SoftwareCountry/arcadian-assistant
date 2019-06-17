namespace Arcadia.Assistant.Organization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    public interface ISupervisorSearch
    {
        Task<EmployeeMetadata> FindAsync(string employeeId, CancellationToken cancellationToken);
    }
}