using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IWorkHoursCredit : IService
    {
        /// <summary>
        /// Returns a number of hours available to take as daysoff. If negative, employee is in debt
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<EmployeeId, int>> GetAvailableHoursAsync(EmployeeId[] employeeIds, CancellationToken cancellationToken);

        Task<WorkHoursChange[]> GetCalendarEventsAsync(EmployeeId employeeId, CancellationToken cancellationToken);

        Task<WorkHoursChange?> GetCalendarEventAsync(EmployeeId employeeId, Guid eventId, CancellationToken cancellationToken);

        Task<WorkHoursChange> RequestChangeAsync(EmployeeId employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart);

        Task<ChangeRequestApproval[]> GetApprovalsAsync(EmployeeId employeeId, Guid eventId, CancellationToken cancellationToken);

        Task ApproveRequestAsync(EmployeeId employeeId, Guid requestId, EmployeeId approvedBy);

        Task RejectRequestAsync(EmployeeId employeeId, Guid requestId, string? rejectionReason, EmployeeId rejectedBy);

        Task CancelRequestAsync(EmployeeId employeeId, Guid requestId, string? rejectionReason, EmployeeId cancelledBy);
    }
}