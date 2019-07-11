using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IWorkHoursCredit : IService
    {
        /// <summary>
        /// Returns a number of hours available to take as daysoff. If negative, employee is in debt
        /// </summary>
        /// <returns></returns>
        Task<Dictionary<string, int>> GetAvailableHoursAsync(string[] employeeIds, CancellationToken cancellationToken);

        Task<WorkHoursChange[]> GetCalendarEventsAsync(string employeeId, CancellationToken cancellationToken);

        Task<WorkHoursChange> GetCalendarEventAsync(string employeeId, Guid eventId, CancellationToken cancellationToken);

        Task<WorkHoursChange> RequestChangeAsync(string employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart);

        Task<ChangeRequestApproval[]> GetApprovalsAsync(string employeeId, Guid eventId, CancellationToken cancellationToken);

        Task ApproveRequestAsync(string employeeId, Guid requestId, string approvedBy);

        Task RejectRequestAsync(string employeeId, Guid requestId, string rejectionReason, string rejectedBy);

        Task CancelRequestAsync(string employeeId, Guid requestId, string rejectionReason, string cancelledBy);
    }
}