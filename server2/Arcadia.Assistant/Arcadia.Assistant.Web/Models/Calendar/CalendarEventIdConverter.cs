namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;

    public class CalendarEventIdConverter
    {
        public string ToDtoId(string calendarEventType, object id)
        {
            return $"{calendarEventType}_{id}";
        }

        public bool TryGetCalendarEventType(string dtoId, out string type)
        {
            type = string.Empty;
            var parts = this.GetIdParts(dtoId);
            if (parts != null)
            {
                type = parts.Value.Item1;
                return true;
            }

            return false;
        }

        public bool TryParseSickLeaveId(string dtoId, out int id)
        {
            id = 0;
            var parts = this.GetIdParts(dtoId);
            return parts != null && parts.Value.Item1 == CalendarEventTypes.Sickleave && int.TryParse(parts.Value.Item2, out id);
        }

        public bool TryParseWorkHoursChangeId(string dtoId, out Guid id)
        {
            id = default;
            var parts = this.GetIdParts(dtoId);
            return parts != null 
                && (parts.Value.Item1 == CalendarEventTypes.Dayoff || parts.Value.Item1 == CalendarEventTypes.Workout)
                && Guid.TryParse(parts.Value.Item2, out id);
        }

        public bool TryParseVacationId(string dtoId, out int id)
        {
            id = default;
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