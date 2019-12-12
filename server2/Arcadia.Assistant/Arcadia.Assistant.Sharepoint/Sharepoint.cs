namespace Arcadia.Assistant.Sharepoint
{
    using System;
    using System.Collections.Generic;
    using System.Fabric;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Calendar.Abstractions;

    using Employees.Contracts;

    using ExternalStorages.Abstractions;

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
        private readonly IOrganization organizations;
        private readonly ISharepointSynchronizationSettings serviceSettings;
        private readonly IEqualityComparer<StorageItem> sharepointStorageItemComparer = new SharepointStorageItemComparer();
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
            ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings)
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
                var sharepointCalendars = departments.Distinct().ToDictionary(x => x,
                    dId => this.GetSharepointCalendarsByDepartment(dId).ToDictionary(x => x,
                        async cal => await this.GetAllSharepointItemsForCalendar(externalStorage, cal)));

                foreach (var departmentId in departments)
                {
                    var departmentEmployes = await this.employees.FindEmployeesAsync(EmployeesQuery.Create().ForDepartment(departmentId), cancellationToken);
                    var employeeIds = departmentEmployes.Select(x => x.EmployeeId).ToArray();

                    var employeeVacations = await this.vacations.GetCalendarEventsByEmployeeAsync(employeeIds, cancellationToken);
                    var employeeVacationValues = employeeVacations.Values.SelectMany(x => x).ToDictionary(x => CspCalendarEventIdParser.GetCalendarEventIdFromCspId(x.VacationId, CalendarEventTypes.Vacation), x => x);

                    var employeeWorkouts = await this.workouts.GetCalendarEventsCollectionAsync(employeeIds, cancellationToken);
                    var employeeWorkoutsValues = employeeWorkouts.Values.SelectMany(x => x).ToDictionary(x => CspCalendarEventIdParser.GetCalendarEventIdFromCspId(x.ChangeId, CalendarEventTypes.Workout), x => x);

                    var employeeSickLeaves = await this.sickLeaves.GetCalendarEventsCollectionAsync(employeeIds, cancellationToken);
                    var employeeSickLeavesValues = employeeSickLeaves.Values.SelectMany(x => x).ToDictionary(x => CspCalendarEventIdParser.GetCalendarEventIdFromCspId(x.SickLeaveId, CalendarEventTypes.Sickleave), x => x);

                    foreach (var calendar in this.GetSharepointCalendarsByDepartment(departmentId))
                    {
                        var storageItems = await sharepointCalendars[departmentId][calendar];

                        var employeeVacationsStorageItems = storageItems.Where(x => x.Category == CalendarEventTypes.Vacation).ToList();
                        var employeeWorkoutsStorageItems = storageItems.Where(x => x.Category == CalendarEventTypes.Workout).ToList();
                        var employeeSickLeavesStorageItems = storageItems.Where(x => x.Category == CalendarEventTypes.Sickleave).ToList();

                        #region vacation synchronization

                        // synchronize vacations for selected department
                        try
                        {
                            var removeStorageItems = employeeVacationsStorageItems.Where(x => !employeeVacationValues.Keys.Contains(x.CalendarEventId)).ToArray();

                            // insert or update items
                            foreach (var vacationEventId in employeeVacationValues.Keys)
                            {
                                var employeeMetadata = departmentEmployes.Single(x => x.EmployeeId == employeeVacationValues[vacationEventId].EmployeeId);
                                await this.UpsertVacation(vacationEventId, calendar, employeeVacationValues[vacationEventId], employeeMetadata, externalStorage, cancellationToken);
                            }

                            // remove redundant items
                            foreach (var item in removeStorageItems)
                            {
                                await externalStorage.DeleteItem(
                                    calendar,
                                    item.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            ServiceEventSource.Current.ServiceMessage(this.Context, e.ToString());
                        }

                        #endregion

                        #region workhours synchronization

                        // synchronize workouts for selected department
                        try
                        {
                            var removeStorageItems = employeeWorkoutsStorageItems.Where(x => !employeeWorkoutsValues.Keys.Contains(x.CalendarEventId)).ToArray();

                            // insert or update items
                            foreach (var workHourEventId in employeeWorkoutsValues.Keys)
                            {
                                var employeeMetadata = departmentEmployes.Single(x => x.EmployeeId == employeeWorkoutsValues[workHourEventId].EmployeeId);
                                await this.UpsertWorkHour(workHourEventId, calendar, employeeWorkoutsValues[workHourEventId], employeeMetadata, externalStorage, cancellationToken);
                            }

                            // remove redundant items
                            foreach (var item in removeStorageItems)
                            {
                                await externalStorage.DeleteItem(
                                    calendar,
                                    item.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            ServiceEventSource.Current.ServiceMessage(this.Context, e.ToString());
                        }

                        #endregion

                        #region sick lives synchronization

                        // synchronize vacations for selected department
                        try
                        {
                            var removeStorageItems = employeeSickLeavesStorageItems.Where(x => !employeeSickLeavesValues.Keys.Contains(x.CalendarEventId)).ToArray();

                            // insert or update items
                            foreach (var sickLeaveEventId in employeeSickLeavesValues.Keys)
                            {
                                var employeeMetadata = departmentEmployes.Single(x => x.EmployeeId == employeeSickLeavesValues[sickLeaveEventId].EmployeeId);
                                await this.UpsertSickLeave(sickLeaveEventId, calendar, employeeSickLeavesValues[sickLeaveEventId], employeeMetadata, externalStorage, cancellationToken);
                            }

                            // remove redundant items
                            foreach (var item in removeStorageItems)
                            {
                                await externalStorage.DeleteItem(
                                    calendar,
                                    item.Id);
                            }
                        }
                        catch (Exception e)
                        {
                            ServiceEventSource.Current.ServiceMessage(this.Context, e.ToString());
                        }

                        #endregion
                    }
                }

#if DEBUG
                await Task.Delay(TimeSpan.FromMinutes(15), cancellationToken);
#else
                await Task.Delay(TimeSpan.FromMinutes(serviceSettings.SynchronizationIntervalMinutes), cancellationToken);
#endif
            }
        }

        private async Task<IEnumerable<string>> GetDepartmentsList(CancellationToken cancellationToken)
        {
            if (this.departmentsCalendarsSettings.DepartmentsCalendars != null && this.departmentsCalendarsSettings.DepartmentsCalendars.Any())
            {
                return this.departmentsCalendarsSettings.DepartmentsCalendars.Select(x => x.DepartmentId);
            }

            return (await this.organizations.GetDepartmentsAsync(cancellationToken)).Select(x => x.DepartmentId.Value.ToString());
        }

        private IEnumerable<string> GetSharepointCalendarsByDepartment(string departmentId)
        {
            return this.departmentsCalendarsSettings.DepartmentsCalendars
                .Where(x => x.DepartmentId == departmentId)
                .Select(x => x.Calendar);
        }

        private async Task<IEnumerable<StorageItem>> GetAllSharepointItemsForCalendar(IExternalStorage externalStorage, string calendar)
        {
            return await externalStorage.GetItems(calendar);
        }

        private async Task<StorageItem> GetSharepointItemForCalendarEvent(IExternalStorage externalStorage, string calendar, string eventId)
        {
            var existingItems = await externalStorage.GetItems(
                calendar,
                new[] { new EqualCondition(x => x.CalendarEventId, eventId) });
            return existingItems.SingleOrDefault();
        }

        private async Task UpsertVacation(string eventId, string calendar, VacationDescription vacation, EmployeeMetadata employeeMetadata, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var datesPeriod = new DatesPeriod(vacation.StartDate.Date, vacation.EndDate.Date);
            var upsertItem = this.CalendarEventToStorageItem(eventId, CalendarEventTypes.Vacation, datesPeriod, employeeMetadata);
            await this.UpsertStorageItem(eventId, calendar, datesPeriod, upsertItem, externalStorage, cancellationToken);
        }

        private async Task UpsertWorkHour(string eventId, string calendar, WorkHoursChange workHours, EmployeeMetadata employeeMetadata, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var datesPeriod = new DatesPeriod(workHours.Date, workHours.Date);
            var upsertItem = this.CalendarEventToStorageItem(eventId, CalendarEventTypes.Workout, datesPeriod, employeeMetadata);
            await this.UpsertStorageItem(eventId, calendar, datesPeriod, upsertItem, externalStorage, cancellationToken);
        }

        private async Task UpsertSickLeave(string eventId, string calendar, SickLeaveDescription sickLeave, EmployeeMetadata employeeMetadata, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var datesPeriod = new DatesPeriod(sickLeave.StartDate.Date, sickLeave.EndDate.Date);
            var upsertItem = this.CalendarEventToStorageItem(eventId, CalendarEventTypes.Sickleave, datesPeriod, employeeMetadata);
            await this.UpsertStorageItem(eventId, calendar, datesPeriod, upsertItem, externalStorage, cancellationToken);
        }

        private async Task UpsertStorageItem(string eventId, string calendar, DatesPeriod datesPeriod, StorageItem upsertItem, IExternalStorage externalStorage, CancellationToken cancellationToken)
        {
            var storageItem = await this.GetSharepointItemForCalendarEvent(externalStorage, calendar, eventId);

            if (storageItem == null)
            {
                await externalStorage.AddItem(
                    calendar,
                    upsertItem,
                    cancellationToken);
            }
            else if (!this.sharepointStorageItemComparer.Equals(upsertItem, storageItem))
            {
                upsertItem.Id = storageItem.Id;
                await externalStorage.UpdateItem(
                    calendar,
                    upsertItem,
                    cancellationToken);
            }
        }

        private StorageItem CalendarEventToStorageItem(string eventId, string calendarEventType, DatesPeriod period, EmployeeMetadata employeeMetadata)
        {
            var totalHours = period.FinishWorkingHour - period.StartWorkingHour;

            var longEventsTitle = $"{employeeMetadata.Name} ({calendarEventType})";
            var shortEventsTitle = $"{employeeMetadata.Name} ({calendarEventType}: {totalHours} hours)";

            var title = calendarEventType == CalendarEventTypes.Vacation || calendarEventType == CalendarEventTypes.Sickleave
                ? longEventsTitle
                : shortEventsTitle;

            var storageItem = new StorageItem
            {
                Title = title,
                StartDate = period.StartDate,
                EndDate = period.EndDate,
                Category = calendarEventType,
                AllDayEvent = true,
                CalendarEventId = eventId
            };

            return storageItem;
        }
    }
}