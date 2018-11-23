namespace Arcadia.Assistant.Web.Health
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Assistant.Health.Abstractions;

    public interface IHealthService
    {
        Task<IDictionary<string, HealthState>> GetHealthState(CancellationToken cancellationToken);
    }
}