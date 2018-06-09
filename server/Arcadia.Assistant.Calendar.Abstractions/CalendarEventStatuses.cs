namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System.Collections.Generic;

    public class CalendarEventStatuses
    {
        private static readonly IReadOnlyDictionary<string, string[]> StatusesByType = new Dictionary<string, string[]>()
            {
                { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.All },
                { CalendarEventTypes.Workout, WorkHoursChangeStatuses.All },
                { CalendarEventTypes.Sickleave, SickLeaveStatuses.All },
                { CalendarEventTypes.Vacation, SickLeaveStatuses.All }
            };

        private static readonly IReadOnlyDictionary<string, string[]> PendingStatusesByType = new Dictionary<string, string[]>()
            {
                { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Pending },
                { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Pending },
                { CalendarEventTypes.Sickleave, SickLeaveStatuses.Pending },
                { CalendarEventTypes.Vacation, SickLeaveStatuses.Pending }
            };

        private static readonly IReadOnlyDictionary<string, string> ApprovedStatusByType = new Dictionary<string, string>()
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Approved },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Approved },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.Approved },
            { CalendarEventTypes.Vacation, SickLeaveStatuses.Approved }
        };

        public string[] AllForType(string type)
        {
            if (StatusesByType.TryGetValue(type, out var statuses))
            {
                return statuses;
            }
            return new string[0];
        }

        public string[] PendingForType(string type)
        {
            if (PendingStatusesByType.TryGetValue(type, out var statuses))
            {
                return statuses;
            }
            return new string[0];
        }

        public string ApproveForType(string type)
        {
            if (ApprovedStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }
    }
}