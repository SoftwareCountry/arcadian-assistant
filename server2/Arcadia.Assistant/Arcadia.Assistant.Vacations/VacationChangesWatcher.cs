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
        private readonly Settings settings;

        public VacationChangesWatcher(Func<Owned<VacationChangesCheck>> vacationChangesCheckFactory, Settings settings)
        {
            this.vacationChangesCheckFactory = vacationChangesCheckFactory;
            this.settings = settings;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    await this.Refresh(cancellationToken).ConfigureAwait(false);
                    this.mres.Wait(this.settings.ChangesCheckInterval, cancellationToken);
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