namespace Arcadia.Assistant.Web.Models.Calendar
{
    using Vacations.Contracts;

    public class VacationsConverter
    {
        private readonly CalendarEventIdConverter idConverter = new CalendarEventIdConverter();

        public CalendarEventModel ToCalendarEvent(VacationDescription vacation)
        {
            return new CalendarEventModel(CalendarEventTypes.Vacation, this.ToDatesPeriod(vacation),
                vacation.Status.ToString());
        }

        public CalendarEventWithIdModel ToCalendarEventWithId(VacationDescription vacation)
        {
            var dtoId = this.idConverter.ToDtoId(CalendarEventTypes.Vacation, vacation.VacationId);
            return new CalendarEventWithIdModel(dtoId, CalendarEventTypes.Vacation, this.ToDatesPeriod(vacation),
                vacation.Status.ToString());
        }

        private DatesPeriodModel ToDatesPeriod(VacationDescription vacationDescription)
        {
            return new DatesPeriodModel
            {
                EndDate = vacationDescription.EndDate,
                StartDate = vacationDescription.StartDate,
                StartWorkingHour = 0,
                FinishWorkingHour = 0
            };
        }
    }
}