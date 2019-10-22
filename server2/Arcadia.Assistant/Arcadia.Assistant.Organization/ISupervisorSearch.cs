namespace Arcadia.Assistant.Organization
{
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    public interface ISupervisorSearch
    {
        Task<EmployeeMetadata?> FindAsync(EmployeeId employeeId, CancellationToken cancellationToken);
    }
}