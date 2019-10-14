namespace Arcadia.Assistant.Web.Models.Calendar
{
    using Vacations.Contracts;

    public class VacationsConverter
    {
        public CalendarEventModel ToCalendarEvent(VacationDescription vacation)
        {
            return new CalendarEventModel(CalendarEventTypes.Vacation, this.ToDatesPeriod(vacation), vacation.Status.ToString());
        }

        public CalendarEventWithIdModel ToCalendarEventWithId(VacationDescription vacation)
        {
            return new CalendarEventWithIdModel(vacation.VacationId.ToString(), CalendarEventTypes.Vacation, this.ToDatesPeriod(vacation), vacation.Status.ToString());
        }

        private DatesPeriodModel ToDatesPeriod(VacationDescription vacationDescription)
        {
            return new DatesPeriodModel()
            {
                EndDate = vacationDescription.EndDate,
                StartDate = vacationDescription.StartDate,
                StartWorkingHour = 0,
                FinishWorkingHour = 0
            };
        }
    }
}