namespace Arcadia.Assistant.Web.UserPreferences
{
    using System.Threading;
    using System.Threading.Tasks;
    using Assistant.UserPreferences;
    using Models;

    public interface IUserPreferencesService
    {
        Task<UserPreferencesModel> GetUserPreferences(string employeeId, CancellationToken cancellationToken);

        Task<SaveUserPreferencesMessage.Response> SaveUserPreferences(string employeeId, UserPreferencesModel userPreferencesModel, CancellationToken cancellationToken);
    }
}