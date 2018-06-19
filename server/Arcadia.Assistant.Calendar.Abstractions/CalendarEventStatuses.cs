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
                { CalendarEventTypes.Vacation, VacationStatuses.All }
            };

        private static readonly IReadOnlyDictionary<string, string[]> PendingStatusesByType = new Dictionary<string, string[]>()
            {
                { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Pending },
                { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Pending },
                { CalendarEventTypes.Sickleave, SickLeaveStatuses.Pending },
                { CalendarEventTypes.Vacation, VacationStatuses.Pending }
            };

        private static readonly IReadOnlyDictionary<string, string> ApprovedStatusByType = new Dictionary<string, string>()
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Approved },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Approved },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.Approved },
            { CalendarEventTypes.Vacation, VacationStatuses.Approved }
        };

        private static readonly IReadOnlyDictionary<string, string> RejectedStatusByType = new Dictionary<string, string>()
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Rejected },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Rejected },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.Rejected },
            { CalendarEventTypes.Vacation, VacationStatuses.Rejected }
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

        public string ApprovedForType(string type)
        {
            if (ApprovedStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }

        public string RejectedForType(string type)
        {
            if (RejectedStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }
    }
}