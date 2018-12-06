namespace Arcadia.Assistant.Organization.Abstractions.EventBus
{
    public class EmployeeMetadataUpdated
    {
        public EmployeeMetadataUpdated(EmployeeMetadata employeeMetadata)
        {
            this.EmployeeMetadata = employeeMetadata;
        }

        public EmployeeMetadata EmployeeMetadata { get; }
    }
}