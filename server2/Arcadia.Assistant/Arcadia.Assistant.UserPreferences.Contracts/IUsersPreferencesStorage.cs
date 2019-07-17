namespace Arcadia.Assistant.UserPreferences.Contracts
{
    using Employees.Contracts;

    public interface IUsersPreferencesStorage
    {
        IUserPreferencesStorage ForEmployee(EmployeeId employeeId);
    }
}