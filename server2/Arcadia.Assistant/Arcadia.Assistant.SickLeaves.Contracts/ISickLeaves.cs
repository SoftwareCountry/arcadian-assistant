using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.SickLeaves.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface ISickLeaves : IService
    {
        Task<SickLeaveDescription[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken);

        Task<Dictionary<EmployeeId, SickLeaveDescription[]>> GetCalendarEventsByEmployeeMapAsync(EmployeeId[] employeeIds, CancellationToken cancellationToken);

        Task<SickLeaveDescription?> GetCalendarEventAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken);

        Task<SickLeaveDescription> CreateSickLeaveAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate);

        Task ProlongSickLeaveAsync(EmployeeId employeeId, int eventId, DateTime endDate);

        Task CancelSickLeaveAsync(EmployeeId employeeId, int eventId, EmployeeId cancelledBy);
    }
}