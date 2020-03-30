namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISharepointRequestExecutor : IDisposable
    {
        Task<T> ExecuteSharepointRequest<T>(SharepointRequest request, CancellationToken cancellationToken = default);

        Task<HttpResponseMessage> ExecuteSharepointRequest(
            SharepointRequest request, CancellationToken cancellationToken = default);
    }
}