using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.SickLeaves.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface ISickLeaves : IService
    {
        Task<object[]> GetCalendarEventsAsync(string employeeId, CancellationToken cancellationToken);

        Task<object> GetCalendarEventAsync(string employeeId, int eventId, CancellationToken cancellationToken);

        Task CreateSickLeaveAsync(string employeeId, DateTime startDate, DateTime endDate);

        Task ProlongSickLeaveAsync(string employeeId, int eventId, DateTime endDate);

        Task CancelSickLeaveAsync(string employeeId, int eventId);
    }
}