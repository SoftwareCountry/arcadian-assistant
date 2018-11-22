namespace Arcadia.Assistant.UserPreferences
{
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Persistence;

    public class UserPreferencesActor : UntypedPersistentActor
    {
        private readonly Dictionary<string, UserPreferences> userPreferences = new Dictionary<string, UserPreferences>();

        public override string PersistenceId => "user-preferences";

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetUserPreferencesMessage msg:
                    var preferences = this.userPreferences.ContainsKey(msg.UserId)
                        ? this.userPreferences[msg.UserId]
                        : null;
                    this.Sender.Tell(new GetUserPreferencesMessage.Response(preferences));
                    break;

                case SaveUserPreferencesMessage msg:
                    this.SaveUserPreferences(msg.UserId, msg.UserPreferences);
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case SaveUserPreferencesMessage msg:
                    this.SaveUserPreferences(msg.UserId, msg.UserPreferences);
                    break;
            }
        }

        private void SaveUserPreferences(string userId, UserPreferences preferences)
        {
            this.userPreferences[userId] = preferences;
        }
    }
}