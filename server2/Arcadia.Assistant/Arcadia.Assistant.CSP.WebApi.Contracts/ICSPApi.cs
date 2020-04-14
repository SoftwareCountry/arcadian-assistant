namespace Arcadia.Assistant.CSP.WebApi.Contracts
{
    using Microsoft.ServiceFabric.Services.Remoting;
    using System.Threading;
    using System.Threading.Tasks;

    using Models;

    public interface ICspApi : IService
    {
        Task<Employee[]> GetEmployees(CancellationToken cancellationToken);

        Task<Department[]> GetDepartments(CancellationToken cancellationToken);

        Task<DepartmentWithPeopleCount[]> GetDepartmentWithPeople(CancellationToken cancellationToken);
    }
}
