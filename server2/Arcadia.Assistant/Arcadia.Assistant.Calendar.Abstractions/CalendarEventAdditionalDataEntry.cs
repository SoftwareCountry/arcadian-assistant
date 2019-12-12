namespace Arcadia.Assistant.Calendar.Abstractions
{
    public class CalendarEventAdditionalDataEntry
    {
        public CalendarEventAdditionalDataEntry(string key, string value)
        {
            this.Key = key;
            this.Value = value;
        }

        public string Key { get; }

        public string Value { get; }
    }
}