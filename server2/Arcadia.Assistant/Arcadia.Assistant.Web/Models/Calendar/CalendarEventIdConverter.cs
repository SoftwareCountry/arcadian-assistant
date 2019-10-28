namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;

    public class CalendarEventIdConverter
    {
        public string ToDtoId(string calendarEventType, object id)
        {
            return $"{calendarEventType}_{id}";
        }

        public bool TryParseSickLeaveId(string dtoId, out int id)
        {
            id = 0;
            var parts = GetIdParts(dtoId);
            return parts != null && parts.Value.Item1 == CalendarEventTypes.Sickleave && int.TryParse(parts.Value.Item2, out id);
        }

        public bool TryParseWorkHoursChangeId(string dtoId, out Guid id)
        {
            id = default(Guid);
            var parts = GetIdParts(dtoId);
            return parts != null 
                && (parts.Value.Item1 == CalendarEventTypes.Dayoff || parts.Value.Item1 == CalendarEventTypes.Workout)
                && Guid.TryParse(parts.Value.Item2, out id);
        }

        public bool TryParseVacationId(string dtoId, out int id)
        {
            id = 0;
            var parts = GetIdParts(dtoId);
            return parts != null && parts.Value.Item1 == CalendarEventTypes.Vacation && int.TryParse(parts.Value.Item2, out id);
        }

        private (string, string)? GetIdParts(string dtoId)
        {
            var parts = dtoId.Split("_");
            if (parts.Length != 2)
            {
                return null;
            }
            else
            {
                return (parts[0], parts[1]);
            }
        }
    }
}