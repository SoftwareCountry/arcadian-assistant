namespace Arcadia.Assistant.VacationsCredit
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
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

        public async Task<Dictionary<string, double>> GetEmailsToDaysMappingAsync(CancellationToken cancellationToken)
        {
            if (!this.configuration.Enabled)
            {
                return null;
            }

            var emailsQuery = EmailSearchQuery.Create()
                .WithSender(this.configuration.Sender)
                .WithSubject(this.configuration.Subject);

            var emails = await this.inbox.GetEmailsAsync(emailsQuery, cancellationToken);
            var lastActualEmail = emails
                .Where(this.CheckEmail)
                .OrderByDescending(e => e.Date)
                .FirstOrDefault();

            if (lastActualEmail == null)
            {
                return null;
            }

            //this.logger.Debug(
            //    $"Received vacations email from {lastActualEmail.Sender} with subject {lastActualEmail.Subject}"
            //);

            var vacationsAttachment = Encoding.UTF8.GetString(lastActualEmail.Attachments.First());
            return this.ParseVacations(vacationsAttachment);
        }

        private Dictionary<string, double> ParseVacations(string vacationsAttachment)
        {
            var lines = vacationsAttachment.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
            var employeeLines = lines
                .Skip(4) // Skip headers
                .Select(l => l.Split(new[] { "\t" }, StringSplitOptions.None))
                .Where(l => l.Length == 2); // All valuable rows contains exactly 2 columns

            var vacations = employeeLines
                .Select(l => new
                {
                    Email = l[0],
                    Vacations = l[1]
                })
                .Where(v => !string.IsNullOrWhiteSpace(v.Email) && !string.IsNullOrWhiteSpace(v.Vacations) && double.TryParse(v.Vacations, out var _))
                .GroupBy(x => x.Email)
                .ToDictionary(x => x.Key, x => double.Parse(x.First().Vacations));
                
            return vacations;
        }

        private bool CheckEmail(Email email)
        {
            return
                email.Sender.Contains(this.configuration.Sender) &&
                email.Subject.Contains(this.configuration.Subject);
        }
    }
}