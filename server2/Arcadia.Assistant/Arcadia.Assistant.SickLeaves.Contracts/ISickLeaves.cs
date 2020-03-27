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

    using Permissions.Contracts;

    public interface ISickLeaves : IService
    {
        SickLeaveDescription[] GetCalendarEvents(EmployeeId employeeId);

        Dictionary<EmployeeId, SickLeaveDescription[]> GetCalendarEventsByEmployeeMap(EmployeeId[] employeeIds);

        SickLeaveDescription GetCalendarEvent(EmployeeId employeeId, int eventId);

        Task<SickLeaveDescription> CreateSickLeaveAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate, UserIdentity userIdentity);

        Task ProlongSickLeaveAsync(EmployeeId employeeId, int eventId, DateTime endDate, UserIdentity userIdentity);

        Task CancelSickLeaveAsync(EmployeeId employeeId, int eventId, UserIdentity cancelledBy);
    }
}