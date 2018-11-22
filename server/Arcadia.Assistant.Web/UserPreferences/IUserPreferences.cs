namespace Arcadia.Assistant.Web.UserPreferences
{
    using System.Threading;
    using System.Threading.Tasks;
    using Models;

    public interface IUserPreferences
    {
        Task<UserPreferencesModel> GetUserPreferences(string userId, CancellationToken cancellationToken);

        Task SaveUserPreferences(string userId, UserPreferencesModel userPreferencesModel, CancellationToken cancellationToken);
    }
}