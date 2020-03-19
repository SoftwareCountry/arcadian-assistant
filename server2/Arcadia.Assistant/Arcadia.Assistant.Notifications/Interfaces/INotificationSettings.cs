namespace Arcadia.Assistant.Notifications.Interfaces
{
    using System.Collections.Generic;

    using Contracts.Models;

    public interface INotificationSettings
    {
        bool EnablePush { get; }

        IReadOnlyDictionary<string, IReadOnlyCollection<NotificationType>> NotificationTemplateProvidersMap { get; }
    }
}