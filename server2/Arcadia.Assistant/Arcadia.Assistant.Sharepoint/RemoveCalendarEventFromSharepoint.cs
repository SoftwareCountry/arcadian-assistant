namespace Arcadia.Assistant.Sharepoint
{
    using Calendar.Abstractions;

    using Employees.Contracts;

    public class RemoveCalendarEventFromSharepoint
    {
        public RemoveCalendarEventFromSharepoint(CalendarEvent @event, EmployeeMetadata employeeMetadata)
        {
            this.Event = @event;
            this.EmployeeMetadata = employeeMetadata;
        }

        public CalendarEvent Event { get; }

        public EmployeeMetadata EmployeeMetadata { get; }
    }
}