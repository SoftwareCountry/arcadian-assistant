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

    public sealed class SharepointSynchronizator
    {
        #region ctor

        public SharepointSynchronizator(ISickLeaves sickLeaves, IVacations vacations, IWorkHoursCredit workouts, ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings, ILogger? logger)
        {
            this.sickLeaves = sickLeaves;
            this.vacations = vacations;
            this.workouts = workouts;
            this.departmentsCalendarsSettings = departmentsCalendarsSettings;
            this.logger = logger;
        }

        #endregion

        #region publci interface

        public async Task Synchronize(IEmployees employees, IEnumerable<string> departments, IExternalStorage storage, CancellationToken cancellationToken)
        {
            var vacationsSync = new EmployeeVacationsSynchronization(storage, this.logger);
            var workoutsSync = new EmployeeWorkoutsSynchronization(storage, this.logger);
            var sickLeavesSync = new EmployeeSickLeavesSynchronization(storage, this.logger);

            var sharepointCalendars = departments.Distinct().ToDictionary(x => x,
                dId => this.GetSharepointCalendarsByDepartment(dId).ToDictionary(x => x,
                    async cal => await this.GetAllSharepointItemsForCalendar(storage, cal)));

            foreach (var departmentId in departments)
            {
                var departmentEmployes = await employees.FindEmployeesAsync(EmployeesQuery.Create().ForDepartment(departmentId), cancellationToken);
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

                    // synchronize vacations for selected department
                    await vacationsSync.SynchronizeItem(calendar, departmentEmployes, employeeVacationValues, storageItems, cancellationToken);

                    // synchronize workouts for selected department
                    await workoutsSync.SynchronizeItem(calendar, departmentEmployes, employeeWorkoutsValues, storageItems, cancellationToken);

                    // synchronize vacations for selected department
                    await sickLeavesSync.SynchronizeItem(calendar, departmentEmployes, employeeSickLeavesValues, storageItems, cancellationToken);
                }
            }
        }

        #endregion

        #region internal class

        private class EmployeeVacationsSynchronization : SharepointItemSynchronization<VacationDescription>
        {
            public EmployeeVacationsSynchronization(IExternalStorage externalStorage, ILogger? logger = null)
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
            public EmployeeWorkoutsSynchronization(IExternalStorage externalStorage, ILogger? logger = null)
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
            public EmployeeSickLeavesSynchronization(IExternalStorage externalStorage, ILogger? logger = null)
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

        #endregion

        #region variables

        private readonly ISickLeaves sickLeaves;
        private readonly IVacations vacations;
        private readonly IWorkHoursCredit workouts;
        private readonly ISharepointDepartmentsCalendarsSettings departmentsCalendarsSettings;
        private readonly ILogger? logger;

        #endregion

        #region private

        private async Task<IEnumerable<StorageItem>> GetAllSharepointItemsForCalendar(IExternalStorage externalStorage, string calendar)
        {
            return await externalStorage.GetItems(calendar);
        }

        private IEnumerable<string> GetSharepointCalendarsByDepartment(string departmentId)
        {
            return this.departmentsCalendarsSettings.DepartmentsCalendars
                .Where(x => x.DepartmentId == departmentId)
                .Select(x => x.Calendar);
        }

        #endregion
    }
}