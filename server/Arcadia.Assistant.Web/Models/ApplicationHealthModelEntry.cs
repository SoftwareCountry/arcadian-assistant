namespace Arcadia.Assistant.Web.Models
{
    public class ApplicationHealthModelEntry
    {
        public ApplicationHealthModelEntry(string stateName, bool stateValue)
        {
            StateName = stateName;
            StateValue = stateValue;
        }

        public string StateName { get; }

        public bool StateValue { get; }
    }
}