namespace Arcadia.Assistant.Notifications.Interfaces
{
    using System.Collections.Generic;

    using Contracts.Models;

    public interface INotificationSettings
    {
        bool EnablePush { get; }

        Dictionary<string, IEnumerable<NotificationType>> ClientNotificationProvidersMap { get; }
    }
}