namespace Arcadia.Assistant.InboxEmail
{
    public class EmailSearchQuery
    {
        private EmailSearchQuery()
        {
        }

        public string Subject { get; private set; }

        public string Sender { get; private set; }

        public uint? MinId { get; private set; }

        public static EmailSearchQuery Create() => new EmailSearchQuery();

        public EmailSearchQuery WithSubject(string subject)
        {
            var query = this.Clone();
            query.Subject = subject;
            return query;
        }

        public EmailSearchQuery WithSender(string sender)
        {
            var query = this.Clone();
            query.Sender = sender;
            return query;
        }

        public EmailSearchQuery FromId(uint id)
        {
            var query = this.Clone();
            this.MinId = id;
            return query;
        }

        private EmailSearchQuery Clone()
        {
            return new EmailSearchQuery
            {
                Subject = this.Subject,
                Sender = this.Sender,
                MinId = this.MinId
            };
        }
    }
}