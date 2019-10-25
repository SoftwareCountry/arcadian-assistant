namespace Arcadia.Assistant.VacationsCredit
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IVacationsDaysLoader
    {
        Task<Dictionary<string, double>?> GetEmailsToDaysMappingAsync(CancellationToken cancellationToken);
    }
}