namespace Arcadia.Assistant.Organization.Abstractions.EventBus
{
    using System.Collections.Generic;

    public class EmployeesMetadataLoaded
    {
        public EmployeesMetadataLoaded(IEnumerable<EmployeeStoredInformation> employees)
        {
            this.Employees = employees;
        }

        public IEnumerable<EmployeeStoredInformation> Employees { get; }
    }
}