namespace Arcadia.Assistant.Web.Models
{
    public class ApplicationHealthModelEntry
    {
        public ApplicationHealthModelEntry(string stateName, bool stateValue, string stateDetails)
        {
            this.StateName = stateName;
            this.StateValue = stateValue;
            this.StateDetails = stateDetails;
        }

        public string StateName { get; }

        public bool StateValue { get; }

        public string StateDetails { get; }
    }
}