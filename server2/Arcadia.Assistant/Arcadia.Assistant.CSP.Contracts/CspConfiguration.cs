namespace Arcadia.Assistant.CSP
{
    public class CspConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;

        public int CompanyId { get; set; }

        public string HeadDepartmentAbbreviation { get; set; } = string.Empty;

        public string UserIdentityDomain { get; set; } = string.Empty;
    }
}