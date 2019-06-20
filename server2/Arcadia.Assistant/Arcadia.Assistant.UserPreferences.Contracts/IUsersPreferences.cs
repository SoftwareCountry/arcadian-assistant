namespace Arcadia.Assistant.UserPreferences.Contracts
{
    public interface IUsersPreferences
    {
        IUserPreferences Get(string userId);
    }
}