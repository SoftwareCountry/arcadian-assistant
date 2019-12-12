﻿namespace Arcadia.Assistant.Calendar.Abstractions
{
    using System;

    public static class CspCalendarEventIdParser
    {
        public static int GetCspIdFromCalendarEvent(string calendarEventId, string calendarEventType)
        {
            var parts = calendarEventId.Split('_');

            if (parts.Length != 2 || parts[0] != calendarEventType || !int.TryParse(parts[1], out var cspId))
            {
                throw new ArgumentException("Calendar event id has wrong format");
            }

            return cspId;
        }

        public static string GetCalendarEventIdFromCspId(int cspId, string calendarEventType)
        {
            return $"{calendarEventType}_{cspId}";
        }

        public static string GetCalendarEventIdFromCspId(Guid cspId, string calendarEventType)
        {
            return $"{calendarEventType}_{cspId}";
        }
    }
}