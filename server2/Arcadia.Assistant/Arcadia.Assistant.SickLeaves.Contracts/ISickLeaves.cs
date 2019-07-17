using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.SickLeaves.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface ISickLeaves : IService
    {
        Task<object[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken);

        Task<object> GetCalendarEventAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken);

        Task CreateSickLeaveAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate);

        Task ProlongSickLeaveAsync(EmployeeId employeeId, int eventId, DateTime endDate);

        Task CancelSickLeaveAsync(EmployeeId employeeId, int eventId);
    }
}