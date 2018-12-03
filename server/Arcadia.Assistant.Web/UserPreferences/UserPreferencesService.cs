namespace Arcadia.Assistant.Web.UserPreferences
{
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;

    using Arcadia.Assistant.UserPreferences;
    using Arcadia.Assistant.Server.Interop;
    using Arcadia.Assistant.Web.Configuration;
    using Arcadia.Assistant.Web.Models;

    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly ITimeoutSettings timeoutSettings;
        private readonly ActorSelection userPreferencesActor;

        public UserPreferencesService(
            IActorRefFactory actorsFactory,
            ActorPathsBuilder actorPathsBuilder,
            ITimeoutSettings timeoutSettings)
        {
            this.userPreferencesActor = actorsFactory.ActorSelection(
                actorPathsBuilder.Get(WellKnownActorPaths.UserPreferences));
            this.timeoutSettings = timeoutSettings;
        }

        public async Task<UserPreferencesModel> GetUserPreferences(string userId, CancellationToken cancellationToken)
        {
            var response = await this.userPreferencesActor.Ask<GetUserPreferencesMessage.Response>(
                new GetUserPreferencesMessage(userId),
                this.timeoutSettings.Timeout,
                cancellationToken);

            return new UserPreferencesModel
            {
                EmailNotifications = response.UserPreferences.EmailNotifications,
                PushNotifications = response.UserPreferences.PushNotifications
            };
        }

        public Task<SaveUserPreferencesMessage.Response> SaveUserPreferences(
            string userId,
            UserPreferencesModel userPreferencesModel,
            CancellationToken cancellationToken)
        {
            var userPreferences = new UserPreferences
            {
                EmailNotifications = userPreferencesModel.EmailNotifications,
                PushNotifications = userPreferencesModel.PushNotifications
            };

            return this.userPreferencesActor.Ask<SaveUserPreferencesMessage.Response>(new SaveUserPreferencesMessage(userId, userPreferences));
        }
    }
}