using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.VacationsCredit.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IVacationsCredit : IService
    {
        Task<int?> GetVacationDaysLeftAsync(string email, CancellationToken cancellationToken);
    }
}