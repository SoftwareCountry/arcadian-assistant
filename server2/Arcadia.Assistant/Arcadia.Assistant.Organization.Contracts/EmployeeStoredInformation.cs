namespace Arcadia.Assistant.Organization.Contracts
{
    public enum Sex { Male, Female, Undefined }

    public class EmployeeStoredInformation
    {
        public EmployeeMetadata Metadata { get; }

        public EmployeeStoredInformation(EmployeeMetadata metadata)
        {
            this.Metadata = metadata;
        }

        public byte[] Photo { get; set; }
    }
}