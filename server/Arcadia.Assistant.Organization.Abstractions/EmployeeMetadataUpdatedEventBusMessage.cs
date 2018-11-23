namespace Arcadia.Assistant.Organization.Abstractions
{
    public class EmployeeMetadataUpdatedEventBusMessage
    {
        public EmployeeMetadataUpdatedEventBusMessage(EmployeeMetadata employeeMetadata)
        {
            this.EmployeeMetadata = employeeMetadata;
        }

        public EmployeeMetadata EmployeeMetadata { get; }
    }
}