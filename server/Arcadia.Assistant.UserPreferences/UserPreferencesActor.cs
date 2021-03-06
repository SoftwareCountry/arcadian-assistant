﻿namespace Arcadia.Assistant.UserPreferences
{
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Persistence;
    using Events;

    public class UserPreferencesActor : UntypedPersistentActor
    {
        private readonly Dictionary<string, UserPreferences> userPreferencesById = new Dictionary<string, UserPreferences>();

        public override string PersistenceId => "user-preferences";

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case GetUserPreferencesMessage msg:
                    this.userPreferencesById.TryGetValue(msg.EmployeeId, out var userPreferences);
                    this.Sender.Tell(new GetUserPreferencesMessage.Response(userPreferences ?? UserPreferences.CreateDefault()));
                    break;

                case SaveUserPreferencesMessage msg:
                    this.HandleSaveUserPreferencesMessage(msg);
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
                case EmailNotificationsPreferenceChangedEvent evt:
                    this.OnEmailNotificationsPreferenceChanged(evt);
                    break;

                case PushNotificationsPreferenceChangedEvent evt:
                    this.OnPushNotificationsPreferenceChanged(evt);
                    break;
            }
        }

        private void HandleSaveUserPreferencesMessage(SaveUserPreferencesMessage message)
        {
            this.userPreferencesById.TryGetValue(message.UserId, out var existingUserPreferences);

            if (existingUserPreferences?.EmailNotifications != message.UserPreferences.EmailNotifications)
            {
                var emailNotificationsEvent = new EmailNotificationsPreferenceChangedEvent(message.UserId, message.UserPreferences.EmailNotifications);
                this.Persist(emailNotificationsEvent, this.OnEmailNotificationsPreferenceChanged);
            }

            if (existingUserPreferences?.PushNotifications != message.UserPreferences.PushNotifications)
            {
                var pushNotificationsEvent = new PushNotificationsPreferenceChangedEvent(message.UserId, message.UserPreferences.PushNotifications);
                this.Persist(pushNotificationsEvent, this.OnPushNotificationsPreferenceChanged);
            }

            this.Sender.Tell(new SaveUserPreferencesMessage.Response());
        }

        private void OnEmailNotificationsPreferenceChanged(EmailNotificationsPreferenceChangedEvent @event)
        {
            this.SetDefaultPreferencesIfNotExists(@event.UserId);
            this.userPreferencesById[@event.UserId].EmailNotifications = @event.EmailNotifications;
        }

        private void OnPushNotificationsPreferenceChanged(PushNotificationsPreferenceChangedEvent @event)
        {
            this.SetDefaultPreferencesIfNotExists(@event.UserId);
            this.userPreferencesById[@event.UserId].PushNotifications = @event.PushNotifications;
        }

        private void SetDefaultPreferencesIfNotExists(string userId)
        {
            if (!this.userPreferencesById.ContainsKey(userId))
            {
                this.userPreferencesById[userId] = UserPreferences.CreateDefault();
            }
        }
    }
}