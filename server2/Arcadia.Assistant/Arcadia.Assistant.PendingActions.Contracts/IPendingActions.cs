using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.PendingActions.Contracts
{
    using Employees.Contracts;
    using Microsoft.ServiceFabric.Services.Remoting;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IPendingActions : IService
    {
        Task<PendingRequests> GetPendingRequestsAsync(EmployeeId employeeId, CancellationToken cancellationToken);
    }
}