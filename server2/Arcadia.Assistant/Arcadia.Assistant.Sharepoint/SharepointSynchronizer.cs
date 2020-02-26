namespace Arcadia.Assistant.Sharepoint
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using CalendarEvent;

    using Employees.Contracts;

    using ExternalStorages.Abstractions;

    using Microsoft.Extensions.Logging;

    using SickLeaves.Contracts;

    using Vacations.Contracts;

    using WorkHoursCredit.Contracts;

    public sealed class SharepointSynchronizer
    {
        private readonly ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings;
        private readonly ILogger logger;
        private readonly ISickLeaves sickLeaves;
        private readonly IVacations vacations;
        private readonly IWorkHoursCredit workouts;

        public SharepointSynchronizer(ISickLeaves sickLeaves, IVacations vacations, IWorkHoursCredit workouts, ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings, ILogger logger)
        {
            this.sickLeaves = sickLeaves;
            this.vacations = vacations;
            this.workouts = workouts;
            this.departmentsCalendarsSettings = departmentsCalendarsSettings;
            this.logger = logger;
        }

        public async Task Synchronize(IEmployees employees, IEnumerable<string> departments, IExternalStorage storage, CancellationToken cancellationToken)
        {
            var vacationsSync = new EmployeeVacationsSynchronization(storage, this.logger);
            var workoutsSync = new EmployeeWorkoutsSynchronization(storage, this.logger);
            var sickLeavesSync = new EmployeeSickLeavesSynchronization(storage, this.logger);

            var storageItemsCache = new Dictionary<string, IEnumerable<StorageItem>>();
            foreach (var departmentId in departments.Distinct())
            {
                var departmentEmployees = await employees.FindEmployeesAsync(EmployeesQuery.Create().ForDepartment(departmentId), cancellationToken);
                var employeeIds = departmentEmployees.Select(x => x.EmployeeId).ToArray();

                var employeeVacationsTask = this.vacations.GetCalendarEventsByEmployeeAsync(employeeIds, cancellationToken);
                var employeeWorkoutsTask = this.workouts.GetCalendarEventsByEmployeeMapAsync(employeeIds, cancellationToken);
                var employeeSickLeavesTask = this.sickLeaves.GetCalendarEventsByEmployeeMapAsync(employeeIds, cancellationToken);
                await Task.WhenAll(employeeVacationsTask, employeeWorkoutsTask, employeeSickLeavesTask);

                var employeeVacationValues = employeeVacationsTask.Result.Values.SelectMany(x => x)
                    .ToDictionary(x => CspCalendarEventIdParser.GetCalendarEventIdFromCspId(x.VacationId, CalendarEventTypes.Vacation), x => x);
                var employeeWorkoutsValues = employeeWorkoutsTask.Result.Values.SelectMany(x => x)
                    .ToDictionary(x => CspCalendarEventIdParser.GetCalendarEventIdFromCspId(x.ChangeId, CalendarEventTypes.Workout), x => x);
                var employeeSickLeavesValues = employeeSickLeavesTask.Result.Values.SelectMany(x => x)
                    .ToDictionary(x => CspCalendarEventIdParser.GetCalendarEventIdFromCspId(x.SickLeaveId, CalendarEventTypes.Sickleave), x => x);

                foreach (var calendar in this.GetSharepointCalendarsByDepartment(departmentId))
                {
                    if (!storageItemsCache.ContainsKey(calendar))
                    {
                        storageItemsCache.Add(calendar, await this.GetAllSharepointItemsForCalendar(storage, calendar));
                    }

                    await Task.WhenAll(
                        // synchronize vacations for selected department
                        vacationsSync.SynchronizeItems(calendar, departmentEmployees, employeeVacationValues, storageItemsCache[calendar], cancellationToken),
                        // synchronize workouts for selected department
                        workoutsSync.SynchronizeItems(calendar, departmentEmployees, employeeWorkoutsValues, storageItemsCache[calendar], cancellationToken),
                        // synchronize vacations for selected department
                        sickLeavesSync.SynchronizeItems(calendar, departmentEmployees, employeeSickLeavesValues, storageItemsCache[calendar], cancellationToken));
                }
            }
        }

        private async Task<IEnumerable<StorageItem>> GetAllSharepointItemsForCalendar(IExternalStorage externalStorage, string calendar)
        {
            return await externalStorage.GetItems(calendar);
        }

        private IEnumerable<string> GetSharepointCalendarsByDepartment(string departmentId)
        {
            return this.departmentsCalendarsSettings.DepartmentsCalendars
                .Where(x => x.DepartmentId == departmentId)
                .Select(x => x.Calendar)
                .Distinct();
        }

        private class EmployeeVacationsSynchronization : SharepointItemSynchronization<VacationDescription>
        {
            public EmployeeVacationsSynchronization(IExternalStorage externalStorage, ILogger logger)
                : base(externalStorage, logger)
            {
            }

            protected override string ItemEventType { get; } = CalendarEventTypes.Vacation;

            protected override DatesPeriod GetItemDatePeriod(VacationDescription item)
            {
                return new DatesPeriod(item.StartDate.Date, item.EndDate.Date);
            }

            protected override EmployeeId GetItemEmployeeId(VacationDescription item)
            {
                return item.EmployeeId;
            }
        }

        private class EmployeeWorkoutsSynchronization : SharepointItemSynchronization<WorkHoursChange>
        {
            public EmployeeWorkoutsSynchronization(IExternalStorage externalStorage, ILogger logger)
                : base(externalStorage, logger)
            {
            }

            protected override string ItemEventType { get; } = CalendarEventTypes.Workout;

            protected override DatesPeriod GetItemDatePeriod(WorkHoursChange item)
            {
                return new DatesPeriod(item.Date, item.Date);
            }

            protected override EmployeeId GetItemEmployeeId(WorkHoursChange item)
            {
                return item.EmployeeId;
            }
        }

        private class EmployeeSickLeavesSynchronization : SharepointItemSynchronization<SickLeaveDescription>
        {
            public EmployeeSickLeavesSynchronization(IExternalStorage externalStorage, ILogger logger)
                : base(externalStorage, logger)
            {
            }

            protected override string ItemEventType { get; } = CalendarEventTypes.Sickleave;

            protected override DatesPeriod GetItemDatePeriod(SickLeaveDescription item)
            {
                return new DatesPeriod(item.StartDate.Date, item.EndDate.Date);
            }

            protected override EmployeeId GetItemEmployeeId(SickLeaveDescription item)
            {
                return item.EmployeeId;
            }
        }
    }
}