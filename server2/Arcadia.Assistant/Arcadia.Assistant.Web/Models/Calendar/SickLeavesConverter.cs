namespace Arcadia.Assistant.Web.Models.Calendar
{
    using SickLeaves.Contracts;

    public class SickLeavesConverter
    {
        public CalendarEventModel ToCalendarEvent(SickLeaveDescription sickLeave)
        {
            return new CalendarEventModel(CalendarEventTypes.Sickleave, this.ToDatesPeriod(sickLeave), sickLeave.Status.ToString());
        }

        public CalendarEventWithIdModel ToCalendarEventWithId(SickLeaveDescription sickLeave)
        {
            return new CalendarEventWithIdModel(sickLeave.SickLeaveId.ToString(), CalendarEventTypes.Sickleave, this.ToDatesPeriod(sickLeave), sickLeave.Status.ToString());
        }

        private DatesPeriodModel ToDatesPeriod(SickLeaveDescription sickLeave)
        {
            return new DatesPeriodModel()
            {
                EndDate = sickLeave.EndDate,
                StartDate = sickLeave.StartDate,
                StartWorkingHour = 0,
                FinishWorkingHour = 8
            };
        }
    }
}