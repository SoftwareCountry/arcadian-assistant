namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Collections.Generic;

    public class EmployeesMetadataLoadedEventBusMessage
    {
        public EmployeesMetadataLoadedEventBusMessage(IEnumerable<EmployeeStoredInformation> employees)
        {
            this.Employees = employees;
        }

        public IEnumerable<EmployeeStoredInformation> Employees { get; }
    }
}