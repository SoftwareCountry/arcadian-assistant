namespace Arcadia.Assistant.UserPreferences.Events
{
    using System.Runtime.Serialization;

    [DataContract]
    public class DependentDepartmentsPendingActionsPreferenceChangedEvent
    {
        public DependentDepartmentsPendingActionsPreferenceChangedEvent(string userId, DependentDepartmentsPendingActions dependentDepartmentsPendingActions)
        {
            this.UserId = userId;
            this.DependentDepartmentsPendingActions = dependentDepartmentsPendingActions;
        }

        [DataMember]
        public string UserId { get; }

        [DataMember]
        public DependentDepartmentsPendingActions DependentDepartmentsPendingActions { get; }
    }
}