using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Employees.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IEmployees : IService
    {
        Task<EmployeeMetadata> FindEmployeeAsync(string employeeId, CancellationToken cancellationToken);

        Task<EmployeeMetadata[]> FindEmployeesAsync(EmployeesQuery employeesQuery, CancellationToken cancellationToken);
    }
}