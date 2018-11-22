namespace Arcadia.Assistant.UserPreferences
{
    using System;
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Persistence;

    public class UserPreferencesActor : UntypedPersistentActor
    {
        private readonly Dictionary<string, UserPreferences> userPreferencesById = new Dictionary<string, UserPreferences>();

        public override string PersistenceId => "user-preferences";

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetUserPreferencesMessage msg:
                    var userPreferences = this.userPreferencesById.ContainsKey(msg.UserId)
                        ? this.userPreferencesById[msg.UserId]
                        : null;
                    this.Sender.Tell(new GetUserPreferencesMessage.Response(userPreferences));
                    break;

                case SaveUserPreferencesMessage msg:
                    try
                    {
                        this.Persist(msg, m =>
                        {
                            this.SaveUserPreferences(msg.UserId, msg.UserPreferences);
                            this.Sender.Tell(new SaveUserPreferencesMessage.Success());
                        });
                    }
                    catch (Exception ex)
                    {
                        this.Sender.Tell(new SaveUserPreferencesMessage.Error(ex.Message));
                    }

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

        private void SaveUserPreferences(string userId, UserPreferences userPreferences)
        {
            this.userPreferencesById[userId] = userPreferences;
        }
    }
}