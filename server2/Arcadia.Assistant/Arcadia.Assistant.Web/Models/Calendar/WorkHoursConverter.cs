namespace Arcadia.Assistant.Web.Models.Calendar
{
    using WorkHoursCredit.Contracts;

    public class WorkHoursConverter
    {
        public CalendarEventModel ToCalendarEvent(WorkHoursChange workHoursChange)
        {
            var type = this.ChangeTypeToString(workHoursChange.ChangeType);
            var dates = this.GetDatesPeriodModel(workHoursChange);

            return new CalendarEventModel(type, dates, workHoursChange.Status);
        }

        public CalendarEventWithIdModel ToCalendarEventWithId(WorkHoursChange workHoursChange)
        {
            return new CalendarEventWithIdModel(
                workHoursChange.ChangeId.ToString(),
                this.ChangeTypeToString(workHoursChange.ChangeType),
                this.GetDatesPeriodModel(workHoursChange),
                workHoursChange.Status);
        }

        private string ChangeTypeToString(WorkHoursChangeType changeType)
        {
            return changeType == WorkHoursChangeType.Dayoff ? CalendarEventTypes.Dayoff : CalendarEventTypes.Workout;
        }

        private DatesPeriodModel GetDatesPeriodModel(WorkHoursChange change)
        {
            return new DatesPeriodModel()
            {
                StartDate = change.Date,
                EndDate = change.Date,
                StartWorkingHour = change.DayPart == DayPart.SecondHalf ? 4 : 0,
                FinishWorkingHour = change.DayPart == DayPart.FirstHalf ? 4 : 8
            };
        }

        public DayPart GetDayPart(DatesPeriodModel dates)
        {
            var dayPart = dates.StartWorkingHour < 4
                ? dates.FinishWorkingHour < 8 ? DayPart.FirstHalf : DayPart.Full
                : DayPart.SecondHalf;

            return dayPart;
        }
    }
}