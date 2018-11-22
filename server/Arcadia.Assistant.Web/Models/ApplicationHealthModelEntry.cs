namespace Arcadia.Assistant.Web.Models
{
    public class ApplicationHealthModelEntry
    {
        public ApplicationHealthModelEntry(string stateName, bool stateValue, string stateDetails)
        {
            StateName = stateName;
            StateValue = stateValue;
            StateDetails = stateDetails;
        }

        public string StateName { get; }

        public bool StateValue { get; }

        public string StateDetails { get; }
    }
}