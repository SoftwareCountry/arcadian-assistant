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
                    if (!this.userPreferencesById.ContainsKey(msg.UserId))
                    {
                        this.Self.Tell(new SaveUserPreferencesMessage(msg.UserId, UserPreferences.Default));
                    }

                    var userPreferences = this.userPreferencesById.ContainsKey(msg.UserId)
                        ? this.userPreferencesById[msg.UserId]
                        : UserPreferences.Default;
                    this.Sender.Tell(new GetUserPreferencesMessage.Response(userPreferences));
                    break;

                case SaveUserPreferencesMessage msg:
                    var @event = new UserChangesPreferencesEvent(msg.UserId, msg.UserPreferences);

                    this.Persist(@event, evt =>
                    {
                        this.OnUserChangesPreferences(evt);
                        this.Sender.Tell(new SaveUserPreferencesMessage.Response());
                    });

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
                case UserChangesPreferencesEvent evt:
                    this.OnUserChangesPreferences(evt);
                    break;
            }
        }

        private void OnUserChangesPreferences(UserChangesPreferencesEvent @event)
        {
            this.SaveUserPreferences(@event.UserId, @event.UserPreferences);
        }

        private void SaveUserPreferences(string userId, UserPreferences userPreferences)
        {
            this.userPreferencesById.TryGetValue(userId, out var existingPreferences);

            if (existingPreferences?.Equals(userPreferences) != true)
            {
                this.userPreferencesById[userId] = userPreferences;
            }
        }
    }
}