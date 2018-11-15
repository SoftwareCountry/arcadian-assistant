namespace Arcadia.Assistant.Web.Health
{
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHealthService
    {
        Task<bool> GetIsServerAlive(CancellationToken cancellationToken);

        Task<bool> GetIs1CAlive(CancellationToken cancellationToken);

        Task<bool> GetIsDatabaseAlive(CancellationToken cancellationToken);
    }
}
