namespace Arcadia.Assistant.Web.Models.Calendar
{
    using System;
    using System.Collections.Generic;

    using SickLeaves.Contracts;

    using WorkHoursCredit.Contracts;

    public class CalendarEventStatuses
    {
        private static readonly IReadOnlyDictionary<string, string[]> StatusesByType = new Dictionary<string, string[]>
        {
            { CalendarEventTypes.Dayoff, Enum.GetNames(typeof(ChangeRequestStatus)) },
            { CalendarEventTypes.Workout, Enum.GetNames(typeof(ChangeRequestStatus)) },
            { CalendarEventTypes.Sickleave, Enum.GetNames(typeof(SickLeaveStatus)) },
            { CalendarEventTypes.Vacation, VacationStatuses.All }
        };

        private static readonly IReadOnlyDictionary<string, string[]> PendingStatusesByType = new Dictionary<string, string[]>
        {
            { CalendarEventTypes.Dayoff, new[] { ChangeRequestStatus.Requested.ToString() } },
            { CalendarEventTypes.Workout, new[] { ChangeRequestStatus.Requested.ToString() } },
            { CalendarEventTypes.Vacation, VacationStatuses.Pending }
        };

        private static readonly IReadOnlyDictionary<string, string[]> ActualStatusesByType = new Dictionary<string, string[]>
        {
            { CalendarEventTypes.Dayoff, new[] { ChangeRequestStatus.Requested.ToString(), ChangeRequestStatus.Approved.ToString() } },
            { CalendarEventTypes.Workout, new[] { ChangeRequestStatus.Requested.ToString(), ChangeRequestStatus.Approved.ToString() } },
            { CalendarEventTypes.Sickleave, new[] { SickLeaveStatus.Requested.ToString(), SickLeaveStatus.Completed.ToString() } },
            { CalendarEventTypes.Vacation, VacationStatuses.Actual }
        };

        private static readonly IReadOnlyDictionary<string, string> ApprovedStatusByType = new Dictionary<string, string>
        {
            { CalendarEventTypes.Dayoff, ChangeRequestStatus.Approved.ToString() },
            { CalendarEventTypes.Workout, ChangeRequestStatus.Approved.ToString() },
            { CalendarEventTypes.Vacation, VacationStatuses.Approved }
        };

        private static readonly IReadOnlyDictionary<string, string> RejectedStatusByType = new Dictionary<string, string>
        {
            { CalendarEventTypes.Dayoff, ChangeRequestStatus.Rejected.ToString() },
            { CalendarEventTypes.Workout, ChangeRequestStatus.Rejected.ToString() },
            { CalendarEventTypes.Vacation, VacationStatuses.Rejected }
        };

        private static readonly IReadOnlyDictionary<string, string> CancelledStatusByType = new Dictionary<string, string>
        {
            { CalendarEventTypes.Dayoff, ChangeRequestStatus.Cancelled.ToString() },
            { CalendarEventTypes.Workout, ChangeRequestStatus.Cancelled.ToString() },
            { CalendarEventTypes.Sickleave, SickLeaveStatus.Cancelled.ToString() },
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

        public string? ApprovedForType(string type)
        {
            if (ApprovedStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }

        public string? RejectedForType(string type)
        {
            if (RejectedStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }

        public string? CancelledForType(string type)
        {
            if (CancelledStatusByType.TryGetValue(type, out var status))
            {
                return status;
            }

            return null;
        }
    }
}