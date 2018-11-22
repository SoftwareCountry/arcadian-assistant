namespace Arcadia.Assistant.Web.UserPreferences
{
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Assistant.UserPreferences;
    using Configuration;
    using Models;
    using Server.Interop;

    public class UserPreferences : IUserPreferences
    {
        private readonly ITimeoutSettings timeoutSettings;
        private readonly ActorSelection userPreferencesActor;

        public UserPreferences(
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

            if (response.UserPreferences == null)
            {
                return UserPreferencesModel.Default;
            }

            return new UserPreferencesModel
            {
                EmailNotifications = response.UserPreferences.EmailNotifications,
                PushNotifications = response.UserPreferences.PushNotifications,
                DependentDepartmentsPendingActions = response.UserPreferences.DependentDepartmentsPendingActions
            };
        }

        public Task SaveUserPreferences(
            string userId,
            UserPreferencesModel userPreferencesModel,
            CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}