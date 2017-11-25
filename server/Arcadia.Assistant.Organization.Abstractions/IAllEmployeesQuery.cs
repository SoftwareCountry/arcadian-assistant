namespace Arcadia.Assistant.Organization.Abstractions
{
    using System.Threading.Tasks;

    public interface IAllEmployeesQuery
    {
        Task<string[]> GetAllEmployeeIds();
    }
}