namespace Arcadia.Assistant.Web.Health
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IHealthService
    {
        Task<IDictionary<string, bool>> GetHealthState(CancellationToken cancellationToken);
    }
}