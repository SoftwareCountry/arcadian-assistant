namespace Arcadia.Assistant.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Employees.Contracts;

    using ExternalStorages.Abstractions;

    using Microsoft.Extensions.Logging;
    using Microsoft.ServiceFabric.Services.Communication.Runtime;
    using Microsoft.ServiceFabric.Services.Runtime;

    using Organization.Contracts;

    using SickLeaves.Contracts;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    /// <summary>
    ///     An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    public class Sharepoint : StatelessService
    {
        private readonly ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings;
        private readonly IEmployees employees;
        private readonly Func<IExternalStorage> externalStorageProvider;
        private readonly ILogger? logger;
        private readonly IOrganization organizations;
        private readonly ISharepointSynchronizationSettings serviceSettings;
        private readonly ISickLeaves sickLeaves;
        private readonly IVacations vacations;
        private readonly IWorkHoursCredit workouts;

        public Sharepoint(
            StatelessServiceContext context,
            IVacations vacations,
            IWorkHoursCredit workouts,
            ISickLeaves sickLeaves,
            IEmployees employees,
            IOrganization organizations,
            Func<IExternalStorage> externalStorageProvider,
            ISharepointSynchronizationSettings serviceSettings,
            ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings,
            ILogger logger)
            : base(context)
        {
            this.externalStorageProvider = externalStorageProvider;
            this.vacations = vacations;
            this.workouts = workouts;
            this.sickLeaves = sickLeaves;
            this.employees = employees;
            this.organizations = organizations;
            this.serviceSettings = serviceSettings;
            this.departmentsCalendarsSettings = departmentsCalendarsSettings;
            this.logger = logger;
        }

        /// <summary>
        ///     Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        /// <summary>
        ///     This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                //ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                // request Sharepoint calendars
                var externalStorage = this.externalStorageProvider();
                var departments = await this.GetDepartmentsList(cancellationToken);
                var synchronizator = new SharepointSynchronizator(this.sickLeaves, this.vacations, this.workouts, this.departmentsCalendarsSettings, this.logger);

                try
                {
                    await synchronizator.Synchronize(this.employees, departments, externalStorage, cancellationToken);
                }
                catch (Exception e)
                {
                    this.logger.LogError(e, "Sharepoint items synchronization fail");
                }

                await Task.Delay(TimeSpan.FromMinutes(this.serviceSettings.SynchronizationIntervalMinutes), cancellationToken);
            }
        }

        private async Task<IEnumerable<string>> GetDepartmentsList(CancellationToken cancellationToken)
        {
            if (this.departmentsCalendarsSettings.DepartmentsCalendars != null && this.departmentsCalendarsSettings.DepartmentsCalendars.Any())
            {
                return this.departmentsCalendarsSettings.DepartmentsCalendars.Select(x => x.DepartmentId).Distinct();
            }

            return (await this.organizations.GetDepartmentsAsync(cancellationToken)).Select(x => x.DepartmentId.Value.ToString());
        }
    }
}