namespace Arcadia.Assistant.Inbox.Contracts
{
    using System.Runtime.Serialization;

    [DataContract]
    public class EmailSearchQuery
    {
        private EmailSearchQuery()
        {
        }

        [DataMember]
        public string? Subject { get; private set; }

        [DataMember]
        public string? Sender { get; private set; }

        [DataMember]
        public uint? MinId { get; private set; }

        [DataMember]
        public uint? LastNEmails { get; private set; }

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
            query.MinId = id;
            return query;
        }

        public EmailSearchQuery TakeLastNEmails(uint lastNEmails)
        {
            var query = this.Clone();
            query.LastNEmails = lastNEmails;
            return query;
        }

        private EmailSearchQuery Clone()
        {
            return new EmailSearchQuery
            {
                Subject = this.Subject,
                Sender = this.Sender,
                MinId = this.MinId,
                LastNEmails = this.LastNEmails
            };
        }

        public override string ToString()
        {
            return $"Subject: {this.Subject}; Sender: {this.Sender}; MinId: {this.MinId}; LastNEmails: {this.LastNEmails}";
        }
    }
}