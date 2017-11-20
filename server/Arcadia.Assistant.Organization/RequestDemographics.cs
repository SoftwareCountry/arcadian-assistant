namespace Arcadia.Assistant.Organization
{
    public class RequestDemographics
    {
        public RequestDemographics(string employeeId)
        {
            this.EmployeeId = employeeId;
        }

        public string EmployeeId { get; }
    }
}