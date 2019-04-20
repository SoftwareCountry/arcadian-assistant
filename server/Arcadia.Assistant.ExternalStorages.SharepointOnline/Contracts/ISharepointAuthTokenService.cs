namespace Arcadia.Assistant.ExternalStorages.SharepointOnline.Contracts
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface ISharepointAuthTokenService
    {
        Task<string> GetAccessToken(string sharepointUrl, CancellationToken cancellationToken = default(CancellationToken));
    }
}