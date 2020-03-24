using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;

[assembly:
    FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2,
        RemotingClientVersion = RemotingClientVersion.V2)]

namespace Arcadia.Assistant.AppCenterBuilds
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IAppCenterNotification
    {
        Task SendNewBuildNotification(
            string buildVersion, string deviceType, CancellationToken cancellationToken);
    }
}