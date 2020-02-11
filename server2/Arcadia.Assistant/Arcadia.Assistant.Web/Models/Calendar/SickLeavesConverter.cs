namespace Arcadia.Assistant.Web.Models.Calendar
{
    using SickLeaves.Contracts;

    public class SickLeavesConverter
    {
        private readonly CalendarEventIdConverter idConverter = new CalendarEventIdConverter();

        public CalendarEventModel ToCalendarEvent(SickLeaveDescription sickLeave)
        {
            return new CalendarEventModel(CalendarEventTypes.Sickleave, this.ToDatesPeriod(sickLeave), sickLeave.Status.ToString());
        }

        public CalendarEventWithIdModel ToCalendarEventWithId(SickLeaveDescription sickLeave)
        {
            var dtoId = this.idConverter.ToDtoId(CalendarEventTypes.Sickleave, sickLeave.SickLeaveId);
            return new CalendarEventWithIdModel(dtoId, CalendarEventTypes.Sickleave, this.ToDatesPeriod(sickLeave), sickLeave.Status.ToString());
        }

        private DatesPeriodModel ToDatesPeriod(SickLeaveDescription sickLeave)
        {
            return new DatesPeriodModel
            {
                EndDate = sickLeave.EndDate,
                StartDate = sickLeave.StartDate,
                StartWorkingHour = 0,
                FinishWorkingHour = 8
            };
        }
    }
}