namespace Arcadia.Assistant.VacationsCredit
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Inbox.Contracts;

    public class VacationsDaysEmailsLoader : IVacationsDaysLoader
    {
        private readonly IInbox inbox;
        private readonly InboxConfiguration configuration;

        public VacationsDaysEmailsLoader(IInbox inbox, InboxConfiguration configuration)
        {
            this.inbox = inbox;
            this.configuration = configuration;
        }

        public Task<Dictionary<string, double>> GetListAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}