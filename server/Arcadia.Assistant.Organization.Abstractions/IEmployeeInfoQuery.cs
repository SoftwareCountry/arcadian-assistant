namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Threading.Tasks;

    public interface IEmployeeInfoQuery
    {
        Task<EmployeeDemographics> GetEmployeeDemographics(string employeeId);
    }
}