namespace Arcadia.Assistant.CSP.Sharepoint
{
    using Arcadia.Assistant.Calendar.Abstractions;
    using Arcadia.Assistant.Organization.Abstractions;

    public class StoreCalendarEventToSharepoint
    {
        public StoreCalendarEventToSharepoint(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            this.Event = @event;
            this.EmployeeMetadata = employeeMetadata;
        }

        public CalendarEvent Event { get; }

        public EmployeeMetadata EmployeeMetadata { get; }
    }
}