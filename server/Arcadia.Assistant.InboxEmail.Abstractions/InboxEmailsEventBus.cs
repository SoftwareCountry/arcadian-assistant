namespace Arcadia.Assistant.InboxEmail.Abstractions
{
    using System.Collections.Generic;

    public class InboxEmailsEventBus
    {
        public InboxEmailsEventBus(IEnumerable<Email> emails)
        {
            this.Emails = emails;
        }

        public IEnumerable<Email> Emails { get; }
    }
}