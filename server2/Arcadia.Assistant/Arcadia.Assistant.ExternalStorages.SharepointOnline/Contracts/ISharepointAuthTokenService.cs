namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISharepointAuthTokenService : IDisposable
    {
        Task<string> GetAccessToken(string sharepointUrl, CancellationToken cancellationToken = default);
    }
}