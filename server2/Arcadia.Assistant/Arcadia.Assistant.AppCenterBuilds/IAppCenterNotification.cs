namespace Arcadia.Assistant.AppCenterBuilds
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAppCenterNotification
    {
        Task Notify(string buildVersion, string deviceType, CancellationToken cancellationToken);
    }
}