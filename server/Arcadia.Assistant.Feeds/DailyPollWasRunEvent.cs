namespace Arcadia.Assistant.Feeds
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class DailyPollWasRunEvent
    {
        [DataMember]
        public DateTime Date { get; set; }

        public DailyPollWasRunEvent(DateTime date)
        {
            this.Date = date;
        }
    }
}