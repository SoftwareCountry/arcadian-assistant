using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2, RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.WorkHoursCredit.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    public interface IWorkHoursCredit : IService
    {
        /// <summary>
        /// Returns a number of hours available to take as daysoff. If negative, employee is in debt
        /// </summary>
        /// <returns></returns>
        Task<int> GetAvailableHoursAsync(string employeeId, CancellationToken cancellationToken);

        Task<Guid> RequestChange(string employeeId, WorkHoursChangeType changeType, DateTime date, DayPart dayPart);

        Task ApproveRequest(Guid requestId, string approvedBy);

        Task RejectRequest(Guid requestId, string rejectionReason, string rejectedBy);

        Task CancelRequest(Guid requestId, string rejectionReason, string cancelledBy);
    }
}