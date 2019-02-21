namespace Arcadia.Assistant.Calendar.Abstractions.EmployeeVacations
{
    public class CheckDatesAvailability
    {
        public CheckDatesAvailability(CalendarEvent @event)
        {
            this.Event = @event;
        }

        public CalendarEvent Event { get; }

        public class Response
        {
            public Response(bool result)
            {
                this.Result = result;
            }

            public bool Result { get; }
        }
    }
}