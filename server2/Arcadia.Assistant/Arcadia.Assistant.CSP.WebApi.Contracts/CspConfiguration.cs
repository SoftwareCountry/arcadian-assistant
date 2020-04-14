namespace Arcadia.Assistant.CSP.WebApi.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CspConfiguration
    {
        [DataMember]
        public string ConnectionString { get; set; } = string.Empty;

        [DataMember]
        public int CompanyId { get; set; }

        [DataMember]
        public string HeadDepartmentAbbreviation { get; set; } = string.Empty;

        [DataMember]
        public string UserIdentityDomain { get; set; } = string.Empty;
    }
}