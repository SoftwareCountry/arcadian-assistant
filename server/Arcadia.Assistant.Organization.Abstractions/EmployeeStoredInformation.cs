namespace Arcadia.Assistant.Organization.Abstractions
{
    using System;

    public enum Sex { Male, Female, Undefined }

    public class EmployeeStoredInformation
    {
        public EmployeeMetadata Metadata { get; }

        public EmployeeStoredInformation(EmployeeMetadata Metadata)
        {
            this.Metadata = Metadata;
        }

        public byte[] Photo { get; set; }
    }
}