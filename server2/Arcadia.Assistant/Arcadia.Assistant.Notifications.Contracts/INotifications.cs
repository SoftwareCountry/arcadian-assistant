namespace Arcadia.Assistant.Notifications.Contracts
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.ServiceFabric.Services.Remoting;

    using Models;

    /// <summary>
    ///     This interface defines the methods exposed by an actor.
    ///     Clients use this interface to interact with the actor that implements it.
    /// </summary>
    public interface INotifications : IService
    {
        Task Send(IEnumerable<string> employeeIds, NotificationType notificationType, CalendarEventType eventType, NotificationMessage notificationMessage, CancellationToken cancellationToken);
    }
}