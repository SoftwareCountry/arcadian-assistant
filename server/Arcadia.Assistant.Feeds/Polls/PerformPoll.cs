namespace Arcadia.Assistant.Feeds.Polls
{
    using System;

    /// <summary>
    /// This message has to be handled by each Polling agent
    /// </summary>
    public class PerformPoll
    {
        public DateTime Date { get; }

        public PerformPoll(DateTime date)
        {
            this.Date = date;
        }
    }
}