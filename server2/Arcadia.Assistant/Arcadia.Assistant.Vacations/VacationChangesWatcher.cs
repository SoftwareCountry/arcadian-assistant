namespace Arcadia.Assistant.Vacations
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Contracts;

    using Employees.Contracts;

    public class VacationChangesWatcher : IDisposable
    {
        private readonly ManualResetEventSlim mres = new ManualResetEventSlim(false);
        private readonly Func<Owned<VacationChangesCheck>> vacationChangesCheckFactory;

        public VacationChangesWatcher(Func<Owned<VacationChangesCheck>> vacationChangesCheckFactory)
        {
            this.vacationChangesCheckFactory = vacationChangesCheckFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await this.Refresh(cancellationToken).ConfigureAwait(false);
                    this.mres.Wait(TimeSpan.FromMinutes(5), cancellationToken);
                    this.mres.Reset();
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public void ForceRefresh()
        {
            this.mres.Reset();
        }

        public void Dispose()
        {
            this.mres.Dispose();
        }

        private async Task Refresh(CancellationToken cancellationToken)
        {
            using (var vacationChangesCheck = this.vacationChangesCheckFactory())
            {
                await vacationChangesCheck.Value.PerformAsync(cancellationToken);
            }
        }
    }
}