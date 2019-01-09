namespace Arcadia.Assistant.InboxEmail.Abstractions
{
    using System.Collections.Generic;

    public class EmailsReceivedEventBus
    {
        public EmailsReceivedEventBus(IEnumerable<Email> emails)
        {
            this.Emails = emails;
        }

        public IEnumerable<Email> Emails { get; }
    }
}