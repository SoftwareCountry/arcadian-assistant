namespace Arcadia.Assistant.InboxEmail
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using MailKit;
    using MailKit.Net.Imap;
    using MailKit.Search;
    using MailKit.Security;
    using MimeKit;

    using Akka.Actor;
    using Akka.Event;

    using Arcadia.Assistant.Configuration.Configuration;
    using Arcadia.Assistant.InboxEmail.Abstractions;

    public class InboxEmailActor : UntypedActor, ILogReceive
    {
        private readonly IImapSettings imapSettings;

        private readonly ILoggingAdapter logger = Context.GetLogger();

        public InboxEmailActor(IImapSettings imapSettings)
        {
            this.imapSettings = imapSettings;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetInboxEmails msg:
                    this.GetEmails(msg.EmailSearchQuery).PipeTo(
                        this.Sender,
                        success: res => new GetInboxEmails.Success(res),
                        failure: err => new GetInboxEmails.Error(err)
                    );
                    break;

                default:
                    this.Unhandled(message);
                    break;
            }
        }

        private async Task<IEnumerable<Email>> GetEmails(EmailSearchQuery query)
        {
            this.logger.Debug("Loading inbox emails started");

            IEnumerable<Email> emails;

            using (var client = new ImapClient())
            {
                await client.ConnectAsync(
                    this.imapSettings.Host,
                    this.imapSettings.Port,
                    this.imapSettings.UseTls ? SecureSocketOptions.StartTls : SecureSocketOptions.None);
                await client.AuthenticateAsync(this.imapSettings.User, this.imapSettings.Password);

                await client.Inbox.OpenAsync(FolderAccess.ReadOnly);

                var inboxQuery = new SearchQuery();

                if (query.Subject != null)
                {
                    inboxQuery = inboxQuery.And(SearchQuery.SubjectContains(query.Subject));
                }

                if (query.Sender != null)
                {
                    inboxQuery = inboxQuery.And(SearchQuery.FromContains(query.Sender));
                }

                var ids = await client.Inbox.SearchAsync(inboxQuery);

                if (query.MinId != null)
                {
                    ids = ids.Where(x => x.Id > query.MinId).ToList();
                }

                var messages = await client.Inbox.FetchAsync(
                    ids,
                    MessageSummaryItems.BodyStructure | MessageSummaryItems.Envelope);
                emails = this.ConvertMessages(client, messages);

                await client.DisconnectAsync(true);
            }

            this.logger.Debug("Loading inbox emails finished");

            return emails;
        }

        private IEnumerable<Email> ConvertMessages(ImapClient client, IEnumerable<IMessageSummary> messages)
        {
            var emails = messages
                .Select(m =>
                {
                    var date = m.Envelope.Date ?? DateTimeOffset.Now;
                    var sender = m.Envelope.From.ToString();
                    var subject = m.NormalizedSubject;

                    var textPart = (TextPart)client.Inbox.GetBodyPart(m.UniqueId, m.TextBody);
                    var text = textPart.Text;

                    var attachments = m.Attachments
                        .Select(a =>
                        {
                            var attachmentPart = client.Inbox.GetBodyPart(m.UniqueId, a);
                            var stream = new MemoryStream();

                            if (attachmentPart is MessagePart messagePart)
                            {
                                messagePart.Message.WriteTo(stream);
                            }
                            else
                            {
                                var mimePart = (MimePart)attachmentPart;
                                mimePart.Content.DecodeTo(stream);
                            }

                            return stream.ToArray();
                        })
                        .ToList();

                    return new Email(m.UniqueId.Id, date, sender, subject, text, attachments);
                })
                .ToList();

            return emails;
        }
    }
}