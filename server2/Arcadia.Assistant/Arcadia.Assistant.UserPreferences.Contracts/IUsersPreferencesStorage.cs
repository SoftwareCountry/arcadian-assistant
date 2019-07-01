namespace Arcadia.Assistant.UserPreferences.Contracts
{
    public interface IUsersPreferencesStorage
    {
        IUserPreferencesStorage ForEmployee(string employeeId);
    }
}