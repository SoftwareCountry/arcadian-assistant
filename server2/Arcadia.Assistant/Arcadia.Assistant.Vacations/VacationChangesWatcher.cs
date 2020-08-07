namespace Arcadia.Assistant.Vacations
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Autofac.Features.OwnedInstances;

    using Microsoft.Extensions.Logging;

    public class VacationChangesWatcher : IDisposable
    {
        private readonly ILogger<VacationChangesWatcher> logger;
        private readonly ManualResetEventSlim mres = new ManualResetEventSlim(false);
        private readonly Settings settings;
        private readonly Func<Owned<VacationChangesCheck>> vacationChangesCheckFactory;

        public VacationChangesWatcher(
            Func<Owned<VacationChangesCheck>> vacationChangesCheckFactory, Settings settings,
            ILogger<VacationChangesWatcher> logger)
        {
            this.vacationChangesCheckFactory = vacationChangesCheckFactory;
            this.settings = settings;
            this.logger = logger;
        }

        public void Dispose()
        {
            this.mres.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    try
                    {
                        await this.Refresh(cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception e)
                    {
                        this.logger.LogError(e, "Error while listening to vacation changes");
                    }

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
            this.mres.Set();
        }

        private async Task Refresh(CancellationToken cancellationToken)
        {
            using var vacationChangesCheck = this.vacationChangesCheckFactory();
            await vacationChangesCheck.Value.PerformAsync(cancellationToken);
        }
    }
}