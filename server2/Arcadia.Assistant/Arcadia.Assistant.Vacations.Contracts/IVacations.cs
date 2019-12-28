using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.Vacations.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IVacations : IService
    {
        Task<VacationDescription[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken);

        Task<Dictionary<EmployeeId, VacationDescription[]>> GetCalendarEventsByEmployeeAsync(EmployeeId[] employeeIds, CancellationToken cancellationToken);

        Task<VacationDescription?> GetCalendarEventAsync(EmployeeId employeeId, int eventId, CancellationToken cancellationToken);

        Task<VacationDescription> RequestVacationAsync(EmployeeId employeeId, DateTime startDate, DateTime endDate);

        Task ChangeDatesAsync(EmployeeId employeeId, int eventId, DateTime startDate, DateTime endDate);

        Task CancelVacationAsync(EmployeeId employeeId, int eventId, EmployeeId cancelledBy, string cancellationReason);

        Task ApproveVacationAsync(EmployeeId employeeId, int eventId, EmployeeId approvedBy);

        Task RejectVacationAsync(EmployeeId employeeId, int eventId, EmployeeId rejectedBy);
    }
}