namespace Arcadia.Assistant.UserPreferences.Contracts
{
    public interface IUsersPreferencesStorage
    {
        IUserPreferencesStorage ForUser(string userId);
    }
}