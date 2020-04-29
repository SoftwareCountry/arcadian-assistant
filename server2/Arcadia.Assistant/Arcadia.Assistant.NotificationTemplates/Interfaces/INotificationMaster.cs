using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Arcadia.Assistant.NotificationTemplates.Interfaces
{
    using System.Threading;

    using Assistant.NotificationTemplates;

    using Employees.Contracts;

    public interface INotificationMaster
    {
        string NotificationSubject { get; }
        
        string NotificationTitle { get; }

        Task<BodyBuilder> GetNotificationMessageBody(
            Event evt, EmployeeId ownerId, EmployeeId approverId, CancellationToken cancellationToken);
    }
}
