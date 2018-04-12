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

        public string[] AllForType(string type)
        {
            if (StatusesByType.TryGetValue(type, out var statuses))
            {
                return statuses;
            }
            return new string[0];
        }
    }
}