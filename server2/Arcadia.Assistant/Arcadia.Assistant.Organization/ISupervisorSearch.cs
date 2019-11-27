namespace Arcadia.Assistant.Organization
{
    using Employees.Contracts;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISupervisorSearch
    {
        Task<EmployeeMetadata?> FindAsync(EmployeeId employeeId, CancellationToken cancellationToken);
    }
}