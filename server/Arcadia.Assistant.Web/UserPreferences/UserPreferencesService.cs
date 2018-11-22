namespace Arcadia.Assistant.Web.UserPreferences
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Assistant.UserPreferences;
    using Configuration;
    using Models;
    using Server.Interop;

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

        public Task<SaveUserPreferencesMessage.Response> SaveUserPreferences(
            string userId,
            UserPreferencesModel userPreferencesModel,
            CancellationToken cancellationToken)
        {
            var userPreferences = new UserPreferences
            {
                EmailNotifications = userPreferencesModel.EmailNotifications,
                PushNotifications = userPreferencesModel.PushNotifications,
                DependentDepartmentsPendingActions = userPreferencesModel.DependentDepartmentsPendingActions
            };

            return this.userPreferencesActor.Ask<SaveUserPreferencesMessage.Response>(new SaveUserPreferencesMessage(userId, userPreferences));
        }
    }
}