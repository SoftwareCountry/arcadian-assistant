namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System.Collections.Generic;

    public class CalendarEventStatuses
    {
        private static readonly IReadOnlyDictionary<string, string[]> StatusesByType = new Dictionary<string, string[]>
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.All },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.All },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.All },
            { CalendarEventTypes.Vacation, VacationStatuses.All }
        };

        private static readonly IReadOnlyDictionary<string, string[]> PendingStatusesByType = new Dictionary<string, string[]>
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Pending },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Pending },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.Pending },
            { CalendarEventTypes.Vacation, VacationStatuses.Pending }
        };

        private static readonly IReadOnlyDictionary<string, string[]> ActualStatusesByType = new Dictionary<string, string[]>
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Actual },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Actual },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.Actual },
            { CalendarEventTypes.Vacation, VacationStatuses.Actual }
        };

        private static readonly IReadOnlyDictionary<string, string> ApprovedStatusByType = new Dictionary<string, string>
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Approved },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Approved },
            { CalendarEventTypes.Vacation, VacationStatuses.Approved }
        };

        private static readonly IReadOnlyDictionary<string, string> RejectedStatusByType = new Dictionary<string, string>
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Rejected },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Rejected },
            { CalendarEventTypes.Vacation, VacationStatuses.Rejected }
        };

        private static readonly IReadOnlyDictionary<string, string> CancelledStatusByType = new Dictionary<string, string>
        {
            { CalendarEventTypes.Dayoff, WorkHoursChangeStatuses.Cancelled },
            { CalendarEventTypes.Workout, WorkHoursChangeStatuses.Cancelled },
            { CalendarEventTypes.Sickleave, SickLeaveStatuses.Cancelled },
            { CalendarEventTypes.Vacation, VacationStatuses.Cancelled }
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

        public string[] ActualForType(string type)
        {
            if (ActualStatusesByType.TryGetValue(type, out var statuses))
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

        public string CancelledForType(string type)
        {
            if (CancelledStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }
    }
}